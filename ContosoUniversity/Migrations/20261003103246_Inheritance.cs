using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContosoUniversity.Migrations
{
    /// <summary>
    /// Tutorial 9: EF scaffolds this migration by dropping the Instructor table and
    /// renaming Student to Person, which would lose the instructor data. As the tutorial
    /// explains, the generated code is replaced with custom SQL that preserves existing
    /// data: the Instructor table is renamed to Person, and the Student rows are copied
    /// into it (with a temporary OldId column so the Enrollment foreign keys can be fixed
    /// up afterwards).
    /// </summary>
    public partial class Inheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The Enrollment FK points at the Student table, which is dropped below.
            // It is recreated at the end of this method, pointing at Person instead.
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Student_StudentID",
                table: "Enrollment");

            // The Instructor table becomes the Person table that holds the whole hierarchy.
            // Foreign keys that reference Instructor (CourseAssignment, Department,
            // OfficeAssignment) follow the rename automatically.
            migrationBuilder.RenameTable(
                name: "Instructor",
                newName: "Person");

            // Columns used by only one of the derived types become nullable.
            // EnrollmentDate does not exist on the renamed table yet, so it is added.
            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentDate",
                table: "Person",
                type: "datetime2",
                nullable: true);

            // Rows that were in the Instructor table are instructors - the default value
            // is applied to them because the column is added as non-nullable.
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Person",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "Instructor");

            // HireDate already exists on the renamed table - it only becomes nullable
            // because the Student rows do not have a hire date.
            migrationBuilder.AlterColumn<DateTime>(
                name: "HireDate",
                table: "Person",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: false);

            // Temporary key used to fix up relationships after copying Student rows.
            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Person",
                type: "int",
                nullable: true);

            // Copy existing Student data into the new Person table.
            migrationBuilder.Sql("INSERT INTO dbo.Person (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) SELECT LastName, FirstName, null AS HireDate, EnrollmentDate, 'Student' AS Discriminator, ID AS OldId FROM dbo.Student");

            // Fix up existing relationships to match the new PK values.
            migrationBuilder.Sql("UPDATE dbo.Enrollment SET StudentID = (SELECT ID FROM dbo.Person WHERE OldId = dbo.Enrollment.StudentID AND Discriminator = 'Student')");

            // Remove temporary key and the now-empty Student table.
            migrationBuilder.DropColumn(
                name: "OldID",
                table: "Person");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Person_StudentID",
                table: "Enrollment",
                column: "StudentID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignment_Person_InstructorID",
                table: "CourseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Person_InstructorID",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Person_StudentID",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_OfficeAssignment_Person_InstructorID",
                table: "OfficeAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "HireDate",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Person");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "Student");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EnrollmentDate",
                table: "Student",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor", x => x.ID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignment_Instructor_InstructorID",
                table: "CourseAssignment",
                column: "InstructorID",
                principalTable: "Instructor",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Instructor_InstructorID",
                table: "Department",
                column: "InstructorID",
                principalTable: "Instructor",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Student_StudentID",
                table: "Enrollment",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfficeAssignment_Instructor_InstructorID",
                table: "OfficeAssignment",
                column: "InstructorID",
                principalTable: "Instructor",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
