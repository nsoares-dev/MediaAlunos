using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Models;

namespace SistemaAvaliacao.Data
{
    public class SistemaAvaliacaoContext : DbContext
    {
        public SistemaAvaliacaoContext (DbContextOptions<SistemaAvaliacaoContext> options)
            : base(options)
        {
        }

        public DbSet<SistemaAvaliacao.Models.Aluno> Aluno { get; set; } = default!;
    }
}
