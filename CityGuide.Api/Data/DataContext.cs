using CityGuide.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityGuide.Api.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}
