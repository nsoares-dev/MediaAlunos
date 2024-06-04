using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SistemaAvaliacao.Models
{
    public class Aluno
    {
        public int AlunoId { get; set; }
        [DisplayName("Nome")]
        public string? NomeAluno { get; set; }
        [DisplayName("Nota de Prova")]
        [Range(1, 10)]
        public double NotaProva { get; set; }
        [DisplayName("Nota de trabalho")]
        [Range(1, 10)]
        public double NotaTrabalho { get; set; }
        [DisplayName("Nota de teste")]
        [Range(1, 10)]
        public double NotaTeste { get; set; }
        [DisplayName("Média")]
        public double Media => (NotaProva + NotaTeste + NotaTrabalho) / 3;
        [DisplayName("Resultado")]
        public string? AvaliacaoFinal { get; set; }
    }
}
