using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrossSolver.API;

namespace CrossSolver.API.Data
{
    public class CrossSolverAPIContext : DbContext
    {
        public CrossSolverAPIContext (DbContextOptions<CrossSolverAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Answer> Answer { get; set; } = default!;
    }
}
