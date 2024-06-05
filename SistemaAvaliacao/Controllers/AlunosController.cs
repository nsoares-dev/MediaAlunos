using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Data;
using SistemaAvaliacao.Models;
using Org.BouncyCastle.Crypto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaAvaliacao.Controllers
{
    public class AlunosController : Controller
    {
        private readonly SistemaAvaliacaoContext _context;

        public AlunosController(SistemaAvaliacaoContext context)
        {
            _context = context;
        }

        // GET: Alunos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Aluno.ToListAsync());
        }

        // GET: Alunos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno
                .FirstOrDefaultAsync(m => m.AlunoId == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        // GET: Alunos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Alunos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlunoId,NomeAluno,NotaProva,NotaTrabalho,NotaTeste")] Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                double media = (aluno.NotaProva + aluno.NotaTrabalho + aluno.NotaTeste) / 3;
                _context.Add(aluno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aluno);
        }

        // GET: Alunos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno.FindAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }
            return View(aluno);
        }

        // POST: Alunos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlunoId,NomeAluno,NotaProva,NotaTrabalho,NotaTeste")] Aluno aluno)
        {
            if (id != aluno.AlunoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aluno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlunoExists(aluno.AlunoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aluno);
        }

        // GET: Alunos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno
                .FirstOrDefaultAsync(m => m.AlunoId == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        // POST: Alunos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aluno = await _context.Aluno.FindAsync(id);
            if (aluno != null)
            {
                _context.Aluno.Remove(aluno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlunoExists(int id)
        {
            return _context.Aluno.Any(e => e.AlunoId == id);
        }

        public async Task<IActionResult> ExportPdf()
        {
            try
            {
                var alunos = await _context.Aluno.ToListAsync();
                string filePath = Path.GetTempFileName() + ".pdf";
                ExportToPdf(filePath, alunos);

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", "Sistema de avaliação.pdf");
            }
            catch (Exception ex)
            {
                return Content($"Erro ao exportar PDF: {ex.Message}");
            }
        }

        public async Task<IActionResult> ExportCsv()
        {
            try
            {
                var alunos = await _context.Aluno.ToListAsync();
                string filePath = Path.GetTempFileName() + ".csv";
                ExportToCsv(filePath, alunos);

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "text/csv", "Sistema de avaliação.csv");
            }
            catch (Exception ex)
            {
                return Content($"Erro ao exportar CSV: {ex.Message}");
            }
        }

        private void ExportToPdf(string filePath, List<Aluno> alunos)
        {
            try
            {
                using (PdfWriter writer = new PdfWriter(filePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        document.Add(new Paragraph("Lista de Alunos").SetBold().SetFontSize(20));

                        document.Add(new Paragraph("\n"));

                        Table table = new Table(6, true);

                        table.AddHeaderCell("Nome");
                        table.AddHeaderCell("Nota de Prova");
                        table.AddHeaderCell("Nota de Trabalho");
                        table.AddHeaderCell("Nota de Teste");
                        table.AddHeaderCell("Média");
                        table.AddHeaderCell("Resultado");

                        foreach (var aluno in alunos)
                        {
                            table.AddCell(aluno.NomeAluno);
                            table.AddCell(aluno.NotaProva.ToString());
                            table.AddCell(aluno.NotaTrabalho.ToString());
                            table.AddCell(aluno.NotaTeste.ToString());
                            table.AddCell(aluno.Media.ToString());
                            table.AddCell(aluno.AvaliacaoFinal);
                        }

                        document.Add(table);

                        document.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar PDF: {ex.Message}", ex);
            }
        }

        private void ExportToCsv(string filePath, List<Aluno> alunos)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(alunos);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar CSV: {ex.Message}", ex);
            }
        }

    }
}
