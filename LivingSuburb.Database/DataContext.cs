using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LivingSuburb.Models;

namespace LivingSuburb.Database
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration configuration;
        public DbSet<Tag> Tags { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<EventTypeCategory> EventTypeCategories { get; set; }        
        public DbSet<EventTag> EventTags { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Suburb> Suburbs { get; set; }
        public DbSet<PostCode> PostCodes { get; set; }
        public DbSet<WeatherCoordinate> Links { get; set; }
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<OurMission> OurMissions { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<JobSubCategory> JobSubCategories { get; set; }
        public DbSet<TagGroup> TagGroups { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobTag> JobTags { get; set; }
        public DbSet<Forex> Forexs { get; set; }
        public DbSet<PreciousMetal> PreciousMetals { get; set; }
        public DbSet<OpenWeather> OpenWeathers { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Temp> Temps { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Gallery> Galleries { get; set; }

        public DataContext(IConfiguration config, DbContextOptions<DataContext> options)
            : base(options)
        {
            configuration = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventCategory>()
            .Property(p => p.Rank)
            .HasDefaultValue(0);

            modelBuilder.Entity<EventCategory>()
            .HasIndex(h => h.Name)
            .IsUnique();

            modelBuilder.Entity<EventType>()
            .Property(p => p.Rank)
            .HasDefaultValue(0);

            modelBuilder.Entity<EventType>()
            .HasIndex(h => h.Name)
            .IsUnique();

            modelBuilder.Entity<EventTypeCategory>()
            .HasKey(p => new { p.EventCategoryId, p.EventTypeId });

            modelBuilder.Entity<EventTag>()
            .HasKey(p => new { p.EventId, p.TagId });

            modelBuilder.Entity<Event>()
            .HasOne(p => p.EventCategory)
            .WithMany(m => m.Events);

            modelBuilder.Entity<Tag>()
            .HasOne(p => p.TagGroup)
            .WithMany(m => m.Tags);

            modelBuilder.Entity<PostCode>()
            .HasKey(p=> new {p.Code,p.SuburbId });

            modelBuilder.Entity<Country>()
            .HasIndex(h => h.Code)
            .IsUnique();

            modelBuilder.Entity<Suburb>()
            .HasOne(p => p.State)
            .WithMany(m => m.Suburbs);

            modelBuilder.Entity<Suburb>()
            .Property(p => p.Population)
            .HasDefaultValue(0);

            modelBuilder.Entity<Suburb>()
            .Property(p => p.Established)
            .HasDefaultValue(DateTime.MinValue);

            modelBuilder.Entity<JobSubCategory>()
            .HasOne(p => p.JobCategory)
            .WithMany(m => m.JobSubCategories);

            modelBuilder.Entity<WeatherCoordinate>()
            .Property(p => p.Enabled)
            .HasDefaultValue(true);

            modelBuilder.Entity<Job>()
            .Property(p => p.PublishedDate)
            .HasDefaultValueSql("getDate()");

            modelBuilder.Entity<Job>()
            .Property(p => p.IsApproved)
            .HasDefaultValue(false);

            modelBuilder.Entity<JobCategory>()
            .Property(p => p.Rank)
            .HasDefaultValue<int>(0);

            modelBuilder.Entity<JobSubCategory>()
            .Property(p => p.Rank)
            .HasDefaultValue<int>(0);

            modelBuilder.Entity<JobTag>()
            .HasKey(p => new { p.JobId, p.TagId });

            modelBuilder.Entity<Forex>()
            .Property(p => p.LastUpdate)
            .HasDefaultValueSql("getDate()");

            modelBuilder.Entity<PreciousMetal>()
            .Property(p => p.LastUpdate)
            .HasDefaultValueSql("getDate()");

            modelBuilder.Entity<OpenWeather>()
            .Property(p => p.LastUpdate)
            .HasDefaultValueSql("getDate()");

            modelBuilder.Entity<Carousel>()
            .Property(p => p.PublishedDate)
            .HasDefaultValueSql("getDate()");
        }
    }
}
