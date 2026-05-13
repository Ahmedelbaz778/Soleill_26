using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTOs.Assessment;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Repositories;

public class AssessmentRepository : IAssessmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssessmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<object>> GetAllFieldsAsync()
    {
        return await _context.QuestionnaireFields
            .Select(f => new { f.Id, f.Name })
            .ToListAsync<object>();
    }

    public async Task<IEnumerable<QuestionResponseDto>> GetQuestionsByFieldAsync(int fieldId)
    {
        return await _context.Questions
            .Where(q => q.FieldId == fieldId)
            .Select(q => new QuestionResponseDto
            {
                Id = q.Id,
                Text = q.Text,
                FieldName = q.Field.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<QuestionResponseDto>> GetAllQuestionsAsync()
    {
        return await _context.Questions
            .Include(q => q.Field)
            .Select(q => new QuestionResponseDto
            {
                Id = q.Id,
                Text = q.Text,
                FieldName = q.Field.Name
            })
            .ToListAsync();
    }

    public async Task SubmitQuestionnaireAsync(IEnumerable<QuestionnaireResult> results)
    {
        await _context.QuestionnaireResults.AddRangeAsync(results);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}