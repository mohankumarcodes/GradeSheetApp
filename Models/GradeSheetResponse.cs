namespace GradeSheetApp.Models
{
    public class GradeSheetResponse
    {
        public double ClassAverage {  get; init; }
        public double StandardDeviation {  get; init; }

        public IEnumerable<StudentResult> Students { get; init; } = [];

    }
}
