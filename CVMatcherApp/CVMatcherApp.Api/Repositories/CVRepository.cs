using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Repositories.Base;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.EntityFrameworkCore;

namespace CVMatcherApp.Api.Repositories;

public class CVRepository : ICVRepository
{
    private readonly MatcherDbContext dbContext;

    public CVRepository(MatcherDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<int> DeleteOldCVsAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-1);
        var oldCVs = await dbContext.CVs.Where(cv => cv.CreatedAt < cutoff).ToListAsync();

        dbContext.RemoveRange(oldCVs);
        await dbContext.SaveChangesAsync();

        return oldCVs.Count;
    }

    public CV ParseCV(string file)
    {
        var cv = new CV
        {
            FileName = file
        };

        using var reader = new PdfReader(file);
        using var doc = new PdfDocument(reader);

        for (int pageNum = 1; pageNum <= doc.GetNumberOfPages(); pageNum++)
        {
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

            string pageText = PdfTextExtractor.GetTextFromPage(doc.GetPage(pageNum), strategy);
            cv.Content = pageText;

            Console.WriteLine($"--- Page {pageNum} ---");
            Console.WriteLine(pageText);
        }

        return cv;
    }
}