using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Controle.Library.Migrations
{
    /// <inheritdoc />
    public partial class CriandoEstruturaInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pastos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    area = table.Column<decimal>(type: "numeric", nullable: false),
                    tipo_pasto = table.Column<int>(type: "integer", nullable: false),
                    capacidade_maxima = table.Column<int>(type: "integer", nullable: false),
                    tem_aguadouro = table.Column<bool>(type: "boolean", nullable: false),
                    dados_aguadouro_data_ultima_afericao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    dados_aguadouro_quantidade_aferida = table.Column<decimal>(type: "numeric", nullable: true),
                    dados_aguadouro_responsavel_afericao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tem_cocho_sal = table.Column<bool>(type: "boolean", nullable: false),
                    dados_cocho_sal_data_ultima_afericao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    dados_cocho_sal_quantidade_aferida = table.Column<decimal>(type: "numeric", nullable: true),
                    dados_cocho_sal_responsavel_afericao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tem_sombra = table.Column<bool>(type: "boolean", nullable: false),
                    ultima_reforma = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    proxima_reforma = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_inclusao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data_ultima_atualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pastos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gados",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pasto_id = table.Column<int>(type: "integer", nullable: false),
                    brinco = table.Column<int>(type: "integer", nullable: false),
                    valido = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    sexo = table.Column<int>(type: "integer", nullable: false),
                    raca = table.Column<int>(type: "integer", nullable: false),
                    data_nascimento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    peso_nascimento = table.Column<decimal>(type: "numeric", nullable: true),
                    peso_atual = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    data_ultima_pesagem = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    pai_id = table.Column<int>(type: "integer", nullable: true),
                    mae_id = table.Column<int>(type: "integer", nullable: true),
                    origem = table.Column<int>(type: "integer", nullable: false),
                    dados_entrada_origem = table.Column<string>(type: "text", nullable: true),
                    dados_entrada_valor_compra = table.Column<long>(type: "bigint", precision: 18, scale: 4, nullable: true),
                    dados_entrada_data = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    finalidade = table.Column<int>(type: "integer", nullable: false),
                    dados_saida_motivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    dados_saida_data = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    prenhez = table.Column<bool>(type: "boolean", nullable: false),
                    dados_prenhez_origem = table.Column<int>(type: "integer", nullable: true),
                    dados_prenhez_data_previsao_parto = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_inclusao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data_ultima_atualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gados", x => x.id);
                    table.ForeignKey(
                        name: "fk_gados_pastos_pasto_id",
                        column: x => x.pasto_id,
                        principalTable: "pastos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movimentacoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gado_id = table.Column<int>(type: "integer", nullable: false),
                    pasto_origem_id = table.Column<int>(type: "integer", nullable: false),
                    pasto_destino_id = table.Column<int>(type: "integer", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    peso_no_momento = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    responsavel_movimentacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_movimentacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimentacoes", x => x.id);
                    table.ForeignKey(
                        name: "fk_movimentacoes_gados_gado_id",
                        column: x => x.gado_id,
                        principalTable: "gados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_movimentacoes_pastos_pasto_destino_id",
                        column: x => x.pasto_destino_id,
                        principalTable: "pastos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_movimentacoes_pastos_pasto_origem_id",
                        column: x => x.pasto_origem_id,
                        principalTable: "pastos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tratamentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gado_id = table.Column<int>(type: "integer", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    nome_produto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fabricante = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    lote = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    data_aplicacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data_proxima_aplicacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    dose = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    tipo_aplicacao = table.Column<int>(type: "integer", nullable: false),
                    responsavel_aplicacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    motivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    observacoes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_inclusao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data_ultima_atualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tratamentos", x => x.id);
                    table.ForeignKey(
                        name: "fk_tratamentos_gados_gado_id",
                        column: x => x.gado_id,
                        principalTable: "gados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_gados_brinco",
                table: "gados",
                column: "brinco",
                unique: true,
                filter: "valido = true");

            migrationBuilder.CreateIndex(
                name: "ix_gados_pasto_id_sexo",
                table: "gados",
                columns: new[] { "pasto_id", "sexo" });

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_gado_id_data_movimentacao",
                table: "movimentacoes",
                columns: new[] { "gado_id", "data_movimentacao" });

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_pasto_destino_id_data_movimentacao",
                table: "movimentacoes",
                columns: new[] { "pasto_destino_id", "data_movimentacao" });

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_pasto_origem_id_data_movimentacao",
                table: "movimentacoes",
                columns: new[] { "pasto_origem_id", "data_movimentacao" });

            migrationBuilder.CreateIndex(
                name: "ix_pastos_nome",
                table: "pastos",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tratamentos_gado_id_data_aplicacao",
                table: "tratamentos",
                columns: new[] { "gado_id", "data_aplicacao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimentacoes");

            migrationBuilder.DropTable(
                name: "tratamentos");

            migrationBuilder.DropTable(
                name: "gados");

            migrationBuilder.DropTable(
                name: "pastos");
        }
    }
}
