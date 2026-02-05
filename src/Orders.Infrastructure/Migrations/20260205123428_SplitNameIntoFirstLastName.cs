using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitNameIntoFirstLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE orders
                SET
                    first_name = NULLIF(BTRIM(REGEXP_REPLACE(customer_name, '\s+.*$', '')), ''),
                    last_name = NULLIF(BTRIM(REGEXP_REPLACE(customer_name, '^\S+\s*', '')), '')
                WHERE
                    customer_name IS NOT NULL
                    AND customer_name <> ''
                    AND (first_name IS NULL OR first_name = '' OR last_name IS NULL OR last_name = '');
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION orders_sync_name_from_customer_name()
                RETURNS TRIGGER AS $$
                BEGIN
                    IF (NEW.first_name IS NULL OR NEW.first_name = '')
                       OR (NEW.last_name IS NULL OR NEW.last_name = '') THEN
                        IF NEW.customer_name IS NOT NULL AND NEW.customer_name <> '' THEN
                            NEW.first_name := NULLIF(BTRIM(REGEXP_REPLACE(NEW.customer_name, '\s+.*$', '')), '');
                            NEW.last_name := NULLIF(BTRIM(REGEXP_REPLACE(NEW.customer_name, '^\S+\s*', '')), '');
                        END IF;
                    END IF;

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION orders_sync_customer_name_from_split()
                RETURNS TRIGGER AS $$
                BEGIN
                    IF NEW.customer_name IS NULL OR NEW.customer_name = '' THEN
                        NEW.customer_name := BTRIM(CONCAT_WS(' ', NEW.first_name, NEW.last_name));
                    END IF;

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                CREATE TRIGGER trg_orders_customer_name_to_split
                BEFORE INSERT OR UPDATE OF customer_name ON orders
                FOR EACH ROW
                EXECUTE FUNCTION orders_sync_name_from_customer_name();
                """);

            migrationBuilder.Sql(
                """
                CREATE TRIGGER trg_orders_split_to_customer_name
                BEFORE INSERT OR UPDATE OF first_name, last_name ON orders
                FOR EACH ROW
                EXECUTE FUNCTION orders_sync_customer_name_from_split();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS trg_orders_split_to_customer_name ON orders;
                DROP TRIGGER IF EXISTS trg_orders_customer_name_to_split ON orders;
                DROP FUNCTION IF EXISTS orders_sync_customer_name_from_split();
                DROP FUNCTION IF EXISTS orders_sync_name_from_customer_name();
                """);

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "orders");
        }
    }
}
