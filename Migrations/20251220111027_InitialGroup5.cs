using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class InitialGroup5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriaAlimento",
                columns: table => new
                {
                    CategoriaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaAlimento", x => x.CategoriaID);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaConsumivel",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaConsumivel", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentStatus",
                columns: table => new
                {
                    EquipmentStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentStatus", x => x.EquipmentStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedor",
                columns: table => new
                {
                    FornecedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeEmpresa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedor", x => x.FornecedorId);
                });

            migrationBuilder.CreateTable(
                name: "LocalizacaoZonaArmazenamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizacaoZonaArmazenamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                columns: table => new
                {
                    ManufacturerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturer", x => x.ManufacturerId);
                });

            migrationBuilder.CreateTable(
                name: "Nurse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfessionalLicense = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nurse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pathology",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pathology", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Receita",
                columns: table => new
                {
                    ReceitaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ModoPreparo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TempoPreparo = table.Column<int>(type: "int", nullable: false),
                    Porcoes = table.Column<int>(type: "int", nullable: false),
                    CaloriasPorPorcao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Proteinas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HidratosCarbono = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gorduras = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsVegetariana = table.Column<bool>(type: "bit", nullable: false),
                    IsVegan = table.Column<bool>(type: "bit", nullable: false),
                    IsLactoseFree = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receita", x => x.ReceitaId);
                });

            migrationBuilder.CreateTable(
                name: "RestricaoAlimentar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Gravidade = table.Column<int>(type: "int", nullable: false),
                    Sintomas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestricaoAlimentar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomLocation",
                columns: table => new
                {
                    RoomLocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomLocation", x => x.RoomLocationId);
                });

            migrationBuilder.CreateTable(
                name: "RoomStatus",
                columns: table => new
                {
                    RoomStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomStatus", x => x.RoomStatusId);
                });

            migrationBuilder.CreateTable(
                name: "RoomType",
                columns: table => new
                {
                    RoomTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomType", x => x.RoomTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Specialty",
                columns: table => new
                {
                    SpecialtyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialty", x => x.SpecialtyId);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumivelID = table.Column<int>(type: "int", nullable: false),
                    ZonaID = table.Column<int>(type: "int", nullable: false),
                    QuantidadeAtual = table.Column<int>(type: "int", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "int", nullable: false),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.StockId);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EstimatedDuration = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeMaterial",
                columns: table => new
                {
                    TypeMaterialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeMaterial", x => x.TypeMaterialID);
                });

            migrationBuilder.CreateTable(
                name: "Alimento",
                columns: table => new
                {
                    AlimentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoriaAlimentoId = table.Column<int>(type: "int", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    KcalPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProteinaGPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HidratosGPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GorduraGPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alimento", x => x.AlimentoId);
                    table.ForeignKey(
                        name: "FK_Alimento_CategoriaAlimento_CategoriaAlimentoId",
                        column: x => x.CategoriaAlimentoId,
                        principalTable: "CategoriaAlimento",
                        principalColumn: "CategoriaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consumivel",
                columns: table => new
                {
                    ConsumivelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuantidadeMaxima = table.Column<int>(type: "int", nullable: false),
                    QuantidadeAtual = table.Column<int>(type: "int", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumivel", x => x.ConsumivelId);
                    table.ForeignKey(
                        name: "FK_Consumivel_CategoriaConsumivel_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriaConsumivel",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentType",
                columns: table => new
                {
                    EquipmentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentType", x => x.EquipmentTypeId);
                    table.ForeignKey(
                        name: "FK_EquipmentType_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomTypeId = table.Column<int>(type: "int", nullable: false),
                    SpecialtyId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoomLocationId = table.Column<int>(type: "int", nullable: false),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RoomStatusId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Room_RoomLocation_RoomLocationId",
                        column: x => x.RoomLocationId,
                        principalTable: "RoomLocation",
                        principalColumn: "RoomLocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_RoomStatus_RoomStatusId",
                        column: x => x.RoomStatusId,
                        principalTable: "RoomStatus",
                        principalColumn: "RoomStatusId");
                    table.ForeignKey(
                        name: "FK_Room_RoomType_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomType",
                        principalColumn: "RoomTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId");
                });

            migrationBuilder.CreateTable(
                name: "TreatmentRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NurseId = table.Column<int>(type: "int", nullable: false),
                    TreatmentTypeId = table.Column<int>(type: "int", nullable: false),
                    PathologyId = table.Column<int>(type: "int", nullable: true),
                    TreatmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EstimatedDuration = table.Column<int>(type: "int", nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedDuration = table.Column<int>(type: "int", nullable: true),
                    CanceledReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentRecord_Nurse_NurseId",
                        column: x => x.NurseId,
                        principalTable: "Nurse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentRecord_Pathology_PathologyId",
                        column: x => x.PathologyId,
                        principalTable: "Pathology",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TreatmentRecord_TreatmentType_TreatmentTypeId",
                        column: x => x.TreatmentTypeId,
                        principalTable: "TreatmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalDevices",
                columns: table => new
                {
                    MedicalDeviceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeMaterialID = table.Column<int>(type: "int", nullable: false),
                    IsUnderMaintenance = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalDevices", x => x.MedicalDeviceID);
                    table.ForeignKey(
                        name: "FK_MedicalDevices_TypeMaterial_TypeMaterialID",
                        column: x => x.TypeMaterialID,
                        principalTable: "TypeMaterial",
                        principalColumn: "TypeMaterialID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypeMaterialHistories",
                columns: table => new
                {
                    TypeMaterialHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeMaterialID = table.Column<int>(type: "int", nullable: false),
                    OldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeMaterialHistories", x => x.TypeMaterialHistoryId);
                    table.ForeignKey(
                        name: "FK_TypeMaterialHistories_TypeMaterial_TypeMaterialID",
                        column: x => x.TypeMaterialID,
                        principalTable: "TypeMaterial",
                        principalColumn: "TypeMaterialID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alergia",
                columns: table => new
                {
                    AlergiaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gravidade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Sintomas = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AlimentoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergia", x => x.AlergiaID);
                    table.ForeignKey(
                        name: "FK_Alergia_Alimento_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimento",
                        principalColumn: "AlimentoId");
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EquipmentTypeId = table.Column<int>(type: "int", nullable: false),
                    EquipmentStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.EquipmentId);
                    table.ForeignKey(
                        name: "FK_Equipment_EquipmentStatus_EquipmentStatusId",
                        column: x => x.EquipmentStatusId,
                        principalTable: "EquipmentStatus",
                        principalColumn: "EquipmentStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equipment_EquipmentType_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalTable: "EquipmentType",
                        principalColumn: "EquipmentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equipment_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomHistories",
                columns: table => new
                {
                    RoomHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Responsible = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsibleId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomHistories", x => x.RoomHistoryId);
                    table.ForeignKey(
                        name: "FK_RoomHistories_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomReservations",
                columns: table => new
                {
                    RoomReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResponsibleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ConsultationId = table.Column<int>(type: "int", nullable: true),
                    ConsultationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    SpecialtyId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomReservations", x => x.RoomReservationId);
                    table.ForeignKey(
                        name: "FK_RoomReservations_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomReservations_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId");
                });

            migrationBuilder.CreateTable(
                name: "ZonaArmazenamento",
                columns: table => new
                {
                    ZonaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeZona = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConsumivelId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CapacidadeMaxima = table.Column<int>(type: "int", nullable: false),
                    QuantidadeAtual = table.Column<int>(type: "int", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonaArmazenamento", x => x.ZonaId);
                    table.ForeignKey(
                        name: "FK_ZonaArmazenamento_Consumivel_ConsumivelId",
                        column: x => x.ConsumivelId,
                        principalTable: "Consumivel",
                        principalColumn: "ConsumivelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZonaArmazenamento_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsoConsumivel",
                columns: table => new
                {
                    UsoConsumivelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TreatmentRecordId = table.Column<int>(type: "int", nullable: false),
                    ConsumivelID = table.Column<int>(type: "int", nullable: false),
                    QuantidadeUsada = table.Column<int>(type: "int", nullable: false),
                    DataConsumo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsoConsumivel", x => x.UsoConsumivelId);
                    table.ForeignKey(
                        name: "FK_UsoConsumivel_Consumivel_ConsumivelID",
                        column: x => x.ConsumivelID,
                        principalTable: "Consumivel",
                        principalColumn: "ConsumivelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsoConsumivel_TreatmentRecord_TreatmentRecordId",
                        column: x => x.TreatmentRecordId,
                        principalTable: "TreatmentRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationMedDevice",
                columns: table => new
                {
                    LocationMedDeviceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalDeviceID = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    InitialDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    RoomReservationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMedDevice", x => x.LocationMedDeviceID);
                    table.ForeignKey(
                        name: "FK_LocationMedDevice_MedicalDevices_MedicalDeviceID",
                        column: x => x.MedicalDeviceID,
                        principalTable: "MedicalDevices",
                        principalColumn: "MedicalDeviceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationMedDevice_RoomReservations_RoomReservationId",
                        column: x => x.RoomReservationId,
                        principalTable: "RoomReservations",
                        principalColumn: "RoomReservationId");
                    table.ForeignKey(
                        name: "FK_LocationMedDevice_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomConsumable",
                columns: table => new
                {
                    RoomConsumableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ConsumivelId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomReservationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomConsumable", x => x.RoomConsumableId);
                    table.ForeignKey(
                        name: "FK_RoomConsumable_Consumivel_ConsumivelId",
                        column: x => x.ConsumivelId,
                        principalTable: "Consumivel",
                        principalColumn: "ConsumivelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomConsumable_RoomReservations_RoomReservationId",
                        column: x => x.RoomReservationId,
                        principalTable: "RoomReservations",
                        principalColumn: "RoomReservationId");
                    table.ForeignKey(
                        name: "FK_RoomConsumable_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alergia_AlimentoId",
                table: "Alergia",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alimento_CategoriaAlimentoId",
                table: "Alimento",
                column: "CategoriaAlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaConsumivel_Nome",
                table: "CategoriaConsumivel",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consumivel_CategoriaId",
                table: "Consumivel",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentStatusId",
                table: "Equipment",
                column: "EquipmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentTypeId",
                table: "Equipment",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_RoomId",
                table: "Equipment",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentType_ManufacturerId",
                table: "EquipmentType",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMedDevice_MedicalDeviceID",
                table: "LocationMedDevice",
                column: "MedicalDeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMedDevice_RoomId",
                table: "LocationMedDevice",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMedDevice_RoomReservationId",
                table: "LocationMedDevice",
                column: "RoomReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_SerialNumber",
                table: "MedicalDevices",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_TypeMaterialID",
                table: "MedicalDevices",
                column: "TypeMaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomLocationId",
                table: "Room",
                column: "RoomLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomStatusId",
                table: "Room",
                column: "RoomStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomTypeId",
                table: "Room",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_SpecialtyId",
                table: "Room",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomConsumable_ConsumivelId",
                table: "RoomConsumable",
                column: "ConsumivelId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomConsumable_RoomId",
                table: "RoomConsumable",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomConsumable_RoomReservationId",
                table: "RoomConsumable",
                column: "RoomReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomHistories_RoomId",
                table: "RoomHistories",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_RoomId",
                table: "RoomReservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_SpecialtyId",
                table: "RoomReservations",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentRecord_NurseId",
                table: "TreatmentRecord",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentRecord_PathologyId",
                table: "TreatmentRecord",
                column: "PathologyId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentRecord_TreatmentTypeId",
                table: "TreatmentRecord",
                column: "TreatmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeMaterialHistories_TypeMaterialID",
                table: "TypeMaterialHistories",
                column: "TypeMaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_UsoConsumivel_ConsumivelID",
                table: "UsoConsumivel",
                column: "ConsumivelID");

            migrationBuilder.CreateIndex(
                name: "IX_UsoConsumivel_TreatmentRecordId",
                table: "UsoConsumivel",
                column: "TreatmentRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ZonaArmazenamento_ConsumivelId",
                table: "ZonaArmazenamento",
                column: "ConsumivelId");

            migrationBuilder.CreateIndex(
                name: "IX_ZonaArmazenamento_RoomId",
                table: "ZonaArmazenamento",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Fornecedor");

            migrationBuilder.DropTable(
                name: "LocalizacaoZonaArmazenamento");

            migrationBuilder.DropTable(
                name: "LocationMedDevice");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "RoomConsumable");

            migrationBuilder.DropTable(
                name: "RoomHistories");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "TypeMaterialHistories");

            migrationBuilder.DropTable(
                name: "UsoConsumivel");

            migrationBuilder.DropTable(
                name: "ZonaArmazenamento");

            migrationBuilder.DropTable(
                name: "Alimento");

            migrationBuilder.DropTable(
                name: "EquipmentStatus");

            migrationBuilder.DropTable(
                name: "EquipmentType");

            migrationBuilder.DropTable(
                name: "MedicalDevices");

            migrationBuilder.DropTable(
                name: "RoomReservations");

            migrationBuilder.DropTable(
                name: "TreatmentRecord");

            migrationBuilder.DropTable(
                name: "Consumivel");

            migrationBuilder.DropTable(
                name: "CategoriaAlimento");

            migrationBuilder.DropTable(
                name: "Manufacturer");

            migrationBuilder.DropTable(
                name: "TypeMaterial");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Nurse");

            migrationBuilder.DropTable(
                name: "Pathology");

            migrationBuilder.DropTable(
                name: "TreatmentType");

            migrationBuilder.DropTable(
                name: "CategoriaConsumivel");

            migrationBuilder.DropTable(
                name: "RoomLocation");

            migrationBuilder.DropTable(
                name: "RoomStatus");

            migrationBuilder.DropTable(
                name: "RoomType");

            migrationBuilder.DropTable(
                name: "Specialty");
        }
    }
}
