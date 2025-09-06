using PsrGameServer.Models;

namespace PsrGameServer.Services;

public interface IQuestionService
{
    Task<List<QuestionAnswer>> GetAllQuestionsAsync();
    Task<QuestionAnswer?> GetQuestionByIdAsync(string id);
    Task<QuestionAnswer> GetRandomQuestionAsync();
    Task<bool> UpdateQuestionsFromJsonAsync(string jsonContent);
    Task<string> GetQuestionsAsJsonAsync();
    Task<bool> LoadQuestionsFromFileAsync();
}