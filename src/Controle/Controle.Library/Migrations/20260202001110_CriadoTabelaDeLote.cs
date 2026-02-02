using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Controle.Library.Migrations
{
    /// <inheritdoc />
    public partial class CriadoTabelaDeLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_gados_pastos_pasto_id",
                table: "gados");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_pastos_pasto_destino_id",
                table: "movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_pastos_pasto_origem_id",
                table: "movimentacoes");

            migrationBuilder.RenameColumn(
                name: "pasto_origem_id",
                table: "movimentacoes",
                newName: "lote_origem_id");

            migrationBuilder.RenameColumn(
                name: "pasto_destino_id",
                table: "movimentacoes",
                newName: "lote_destino_id");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_pasto_origem_id_data_movimentacao",
                table: "movimentacoes",
                newName: "ix_movimentacoes_lote_origem_id_data");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_pasto_destino_id_data_movimentacao",
                table: "movimentacoes",
                newName: "ix_movimentacoes_lote_destino_id_data");

            migrationBuilder.RenameColumn(
                name: "pasto_id",
                table: "gados",
                newName: "lote_id");

            migrationBuilder.RenameIndex(
                name: "ix_gados_pasto_id_sexo",
                table: "gados",
                newName: "ix_gados_lote_id_sexo");

            migrationBuilder.AddColumn<int>(
                name: "lote_origem_id1",
                table: "movimentacoes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "lotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pasto_id = table.Column<int>(type: "integer", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    area = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    capacidade_maxima = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_inclusao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data_ultima_atualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lotes", x => x.id);
                    table.ForeignKey(
                        name: "fk_lotes_pastos_pasto_id",
                        column: x => x.pasto_id,
                        principalTable: "pastos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_lote_origem_id1",
                table: "movimentacoes",
                column: "lote_origem_id1");

            migrationBuilder.CreateIndex(
                name: "ix_lotes_pasto_id_numero",
                table: "lotes",
                columns: new[] { "pasto_id", "numero" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_gados_lotes_lote_id",
                table: "gados",
                column: "lote_id",
                principalTable: "lotes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_lotes_lote_origem_id",
                table: "movimentacoes",
                column: "lote_origem_id",
                principalTable: "lotes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_lotes_lote_origem_id1",
                table: "movimentacoes",
                column: "lote_origem_id1",
                principalTable: "lotes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_gados_lotes_lote_id",
                table: "gados");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_lotes_lote_origem_id",
                table: "movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_lotes_lote_origem_id1",
                table: "movimentacoes");

            migrationBuilder.DropTable(
                name: "lotes");

            migrationBuilder.DropIndex(
                name: "ix_movimentacoes_lote_origem_id1",
                table: "movimentacoes");

            migrationBuilder.DropColumn(
                name: "lote_origem_id1",
                table: "movimentacoes");

            migrationBuilder.RenameColumn(
                name: "lote_origem_id",
                table: "movimentacoes",
                newName: "pasto_origem_id");

            migrationBuilder.RenameColumn(
                name: "lote_destino_id",
                table: "movimentacoes",
                newName: "pasto_destino_id");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_lote_origem_id_data",
                table: "movimentacoes",
                newName: "ix_movimentacoes_pasto_origem_id_data_movimentacao");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_lote_destino_id_data",
                table: "movimentacoes",
                newName: "ix_movimentacoes_pasto_destino_id_data_movimentacao");

            migrationBuilder.RenameColumn(
                name: "lote_id",
                table: "gados",
                newName: "pasto_id");

            migrationBuilder.RenameIndex(
                name: "ix_gados_lote_id_sexo",
                table: "gados",
                newName: "ix_gados_pasto_id_sexo");

            migrationBuilder.AddForeignKey(
                name: "fk_gados_pastos_pasto_id",
                table: "gados",
                column: "pasto_id",
                principalTable: "pastos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_pastos_pasto_destino_id",
                table: "movimentacoes",
                column: "pasto_destino_id",
                principalTable: "pastos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_pastos_pasto_origem_id",
                table: "movimentacoes",
                column: "pasto_origem_id",
                principalTable: "pastos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
