using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SurveyApi.Models;

public partial class DbSurveyProjectContext(IConfiguration configuration, IOptions<KeyVaultOptions> options) : DbContext
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IOptions<KeyVaultOptions> _options = options;

    // Commented out to inject Configuration
    //public DbSurveyProjectContext(DbContextOptions<DbSurveyProjectContext> options)
    //    : base(options)
    //{
    //}

    public virtual DbSet<AnswersInSurvey> AnswersInSurveys { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveyAnswer> SurveyAnswers { get; set; }

    public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }

    public virtual DbSet<SurveyStatus> SurveyStatuses { get; set; }

    public virtual DbSet<SurveyType> SurveyTypes { get; set; }

    // Build connection string without revealing secret
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new(_configuration.GetConnectionString("SurveyDbConnectionString"))
        {
            Password = _options.Value.DbAdminPassword
        };

        optionsBuilder.UseSqlServer(sqlConnectionStringBuilder.ToString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnswersInSurvey>(entity =>
        {
            entity.HasKey(e => e.AnswersInSurveysId);

            entity.HasOne(d => d.SurveyAnswer).WithMany(p => p.AnswersInSurveys)
                .HasForeignKey(d => d.SurveyAnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnswersInSurveys_SurveyAnswer");

            entity.HasOne(d => d.Survey).WithMany(p => p.AnswersInSurveys)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnswersInSurveys_Survey");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.ToTable("Survey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.SurveyStatus).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.SurveyStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Survey_SurveyStatus");

            entity.HasOne(d => d.SurveyType).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.SurveyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Survey_SurveyType");
        });

        modelBuilder.Entity<SurveyAnswer>(entity =>
        {
            entity.ToTable("SurveyAnswer");

            entity.Property(e => e.AnswerText)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyAnswers)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyAnswer_Survey");

            entity.HasOne(d => d.SurveyQuestion).WithMany(p => p.SurveyAnswers)
                .HasForeignKey(d => d.SurveyQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyAnswer_SurveyQuestion");
        });

        modelBuilder.Entity<SurveyQuestion>(entity =>
        {
            entity.ToTable("SurveyQuestion");

            entity.Property(e => e.QuestionText)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.SurveyType).WithMany(p => p.SurveyQuestions)
                .HasForeignKey(d => d.SurveyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyQuestion_SurveyType");
        });

        modelBuilder.Entity<SurveyStatus>(entity =>
        {
            entity.ToTable("SurveyStatus");

            entity.Property(e => e.SurveyStatus1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SurveyStatus");
        });

        modelBuilder.Entity<SurveyType>(entity =>
        {
            entity.ToTable("SurveyType");

            entity.Property(e => e.Origin)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
