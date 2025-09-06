using System.Text.Json;
using System.Text.Json.Serialization;
using PsrGameServer.Models;

namespace PsrGameServer.Services;

public class QuestionService : IQuestionService
{
    private readonly List<QuestionAnswer> _questions = new();
    private readonly object _lock = new();
    private readonly Random _random = new();
    private readonly string _questionsFilePath;
    private readonly ILogger<QuestionService> _logger;

    public QuestionService(ILogger<QuestionService> logger)
    {
        _logger = logger;
        _questionsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "questions.json");
        _ = LoadQuestionsFromFileAsync();
    }

    public async Task<List<QuestionAnswer>> GetAllQuestionsAsync()
    {
        lock (_lock)
        {
            return new List<QuestionAnswer>(_questions);
        }
    }

    public async Task<QuestionAnswer?> GetQuestionByIdAsync(string id)
    {
        lock (_lock)
        {
            return _questions.FirstOrDefault(q => q.Id == id);
        }
    }

    public async Task<QuestionAnswer> GetRandomQuestionAsync()
    {
        lock (_lock)
        {
            if (_questions.Count == 0)
            {
                return new QuestionAnswer("What is 2+2?", "4"); // Fallback question
            }
            return _questions[_random.Next(_questions.Count)];
        }
    }

    public async Task<bool> UpdateQuestionsFromJsonAsync(string jsonContent)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var questionData = JsonSerializer.Deserialize<QuestionContainer>(jsonContent, options);
            if (questionData?.Questions == null || questionData.Questions.Count == 0)
            {
                _logger.LogWarning("No questions found in provided JSON");
                return false;
            }

            // Validate questions
            foreach (var question in questionData.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.Question) || string.IsNullOrWhiteSpace(question.Answer))
                {
                    _logger.LogWarning("Invalid question found: empty question or answer");
                    return false;
                }
                
                if (string.IsNullOrWhiteSpace(question.Id))
                {
                    question.Id = Guid.NewGuid().ToString();
                }
            }

            lock (_lock)
            {
                _questions.Clear();
                _questions.AddRange(questionData.Questions);
            }

            // Save to file
            await File.WriteAllTextAsync(_questionsFilePath, jsonContent);
            _logger.LogInformation($"Successfully updated {questionData.Questions.Count} questions");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating questions from JSON");
            return false;
        }
    }

    public async Task<string> GetQuestionsAsJsonAsync()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            QuestionContainer container;
            lock (_lock)
            {
                container = new QuestionContainer { Questions = new List<QuestionAnswer>(_questions) };
            }

            return JsonSerializer.Serialize(container, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serializing questions to JSON");
            return "{}";
        }
    }

    public async Task<bool> LoadQuestionsFromFileAsync()
    {
        try
        {
            if (!File.Exists(_questionsFilePath))
            {
                _logger.LogWarning($"Questions file not found at {_questionsFilePath}");
                await LoadDefaultQuestionsAsync();
                return true;
            }

            var jsonContent = await File.ReadAllTextAsync(_questionsFilePath);
            return await UpdateQuestionsFromJsonAsync(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading questions from file");
            await LoadDefaultQuestionsAsync();
            return false;
        }
    }

    private async Task LoadDefaultQuestionsAsync()
    {
        _logger.LogInformation("Loading default questions");
        var defaultQuestions = new List<QuestionAnswer>
        {
            new("What is 2+2?", "4", QuestionType.Text),
            new("What is the capital of Australia?", "Canberra", QuestionType.Text),
            new("What is the largest planet in our solar system?", "Jupiter", QuestionType.Text),
            new("What year did World War II end?", "1945", QuestionType.Text),
            new("What is the chemical symbol for gold?", "Au", QuestionType.Text)
        };

        lock (_lock)
        {
            _questions.Clear();
            _questions.AddRange(defaultQuestions);
        }
    }

    private class QuestionContainer
    {
        public List<QuestionAnswer> Questions { get; set; } = new();
    }
}