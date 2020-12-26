using Microsoft.EntityFrameworkCore.Migrations;

namespace AMZEnterpriseWebsite.Data.Migrations
{
    public partial class AddWalletToSettingModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BitcoinWalletKey",
                table: "Settings",
                newName: "WalletName9");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "Settings",
                newName: "WalletName8");

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress1",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress10",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress2",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress3",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress4",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress5",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress6",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress7",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress8",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress9",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName1",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName10",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName2",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName3",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName4",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName5",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName6",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletName7",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WalletAddress1",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress10",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress2",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress3",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress4",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress5",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress6",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress7",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress8",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletAddress9",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName1",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName10",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName2",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName3",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName4",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName5",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName6",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "WalletName7",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "WalletName9",
                table: "Settings",
                newName: "BitcoinWalletKey");

            migrationBuilder.RenameColumn(
                name: "WalletName8",
                table: "Settings",
                newName: "BankId");
        }
    }
}
