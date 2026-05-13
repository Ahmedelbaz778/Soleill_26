namespace Soleil.Models.DTOs.Assessment;

public class SubmitQuestionnaireDto
{
    public int ChildId { get; set; }
    public List<AnswerDto> Answers { get; set; } = new();
}

public class AnswerDto
{
    public int QuestionId { get; set; }
    public int Score { get; set; } // الدرجة (مثلاً: 0=أبداً، 1=نادراً، 2=أحياناً، 3=دائماً)
}