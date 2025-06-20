using System.Linq;
using CsvHelper.Configuration;
using CsvHelper;
using GradeSheetApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Globalization;

namespace GradeSheetApp.Services
{
    public interface IGradeCalculator
    {
        Task<GradeSheetResponse> CalculateAsync(Stream csvStream, bool excludeLowest, CancellationToken ct);
    }

    public class GradeCalculator : IGradeCalculator
    {
        private readonly SortedDictionary<string, int> _boundaries; // high‑to‑low for easy lookup

        public GradeCalculator(IConfiguration config)
        {
            // 1. Pull the grade thresholds from appsettings
            var gradeBoundaries = config.GetSection("LetterGrades")
                                        .Get<Dictionary<string, int>>()!;

            // 2. Sort them from highest → lowest percentage
            var ordered = gradeBoundaries.OrderByDescending(kv => kv.Value);

            // 3. Insert into an empty SortedDictionary
            _boundaries = new SortedDictionary<string, int>();
            foreach (var kv in ordered)
                _boundaries.Add(kv.Key, kv.Value);
        }

        public async Task<GradeSheetResponse> CalculateAsync(Stream csvStream,
                                                       bool excludeLowest,
                                                       CancellationToken ct)
        { 
            
            using var csvReader = new StreamReader(csvStream);
            using var csv = new CsvReader(csvReader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                MissingFieldFound = null,
                HeaderValidated = null,
                DetectDelimiter = true,
                PrepareHeaderForMatch = h => h.Header.ToLowerInvariant()
            });

            var rows = await csv.GetRecordsAsync<dynamic>(ct).ToListAsync(ct);

            if (!rows.Any()) throw new InvalidDataException("CSV is empty.");

            var students = new List<StudentResult>();

            foreach (IDictionary<string, object?> row in rows)
            {
                // first column assumed to be "studentId" (case‑insensitive)
                var (idKey, idVal) = row.First();
                var studentId = idVal?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(studentId))
                    throw new InvalidDataException("Missing student ID.");

                // remaining columns are marks
                var rawScores = row.Skip(1)
                                   .Select(p =>
                                   {
                                       if (!double.TryParse(p.Value?.ToString(), out var m))
                                           throw new InvalidDataException($"Non‑numeric score for {studentId}.");
                                       return m;
                                   })
                                   .ToList();

                if (!rawScores.Any()) throw new InvalidDataException($"No scores for {studentId}.");

                if (excludeLowest && rawScores.Count > 1)
                    rawScores.Remove(rawScores.Min());

                var percentage = rawScores.Sum() / rawScores.Count;
                var letter = ResolveLetter(percentage);

                students.Add(new(studentId, Math.Round(percentage, 2), letter));
            }

            // stats
            var pctArray = students.Select(s => s.Percentage).ToArray();
            var avg = pctArray.Average();
            var variance = pctArray.Select(p => Math.Pow(p - avg, 2)).Average();
            var stdDev = Math.Sqrt(variance);

            return new GradeSheetResponse
            {
                ClassAverage = Math.Round(avg, 2),
                StandardDeviation = Math.Round(stdDev, 2),
                Students = students.OrderByDescending(s => s.Percentage)
            };
        }

        private string ResolveLetter(double pct)
            => _boundaries.FirstOrDefault(b => pct >= b.Value).Key ?? "F";


        }
}
