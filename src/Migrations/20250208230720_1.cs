using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeClone.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimeSeasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Season = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeSeasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Durations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Durations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Episodes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationId = table.Column<int>(type: "int", nullable: true),
                    AnimeSeasonId = table.Column<int>(type: "int", nullable: true),
                    Sources = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Synonyms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedAnime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animes_AnimeSeasons_AnimeSeasonId",
                        column: x => x.AnimeSeasonId,
                        principalTable: "AnimeSeasons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Animes_Durations_DurationId",
                        column: x => x.DurationId,
                        principalTable: "Durations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AnimeTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnimeId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimeTag_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimeTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animes_AnimeSeasonId",
                table: "Animes",
                column: "AnimeSeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Animes_DurationId",
                table: "Animes",
                column: "DurationId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeTag_AnimeId",
                table: "AnimeTag",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeTag_TagId",
                table: "AnimeTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimeTag");

            migrationBuilder.DropTable(
                name: "Animes");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "AnimeSeasons");

            migrationBuilder.DropTable(
                name: "Durations");
        }
    }
}
