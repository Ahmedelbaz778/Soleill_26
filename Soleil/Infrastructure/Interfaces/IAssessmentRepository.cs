using Soleil.Models.DTOs.Assessment;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Interfaces;

public interface IAssessmentRepository
{
    Task<IEnumerable<object>> GetAllFieldsAsync();
    Task<IEnumerable<QuestionResponseDto>> GetQuestionsByFieldAsync(int fieldId);
    Task<IEnumerable<QuestionResponseDto>> GetAllQuestionsAsync();
    Task SubmitQuestionnaireAsync(IEnumerable<QuestionnaireResult> results);
    Task SaveChangesAsync();
}