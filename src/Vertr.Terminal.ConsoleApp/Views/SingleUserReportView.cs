using Spectre.Console;
using Vertr.Exchange.Shared.Reports;
using Vertr.Exchange.Shared.Reports.Dtos;
using Vertr.Terminal.ApiClient.Extensions;

namespace Vertr.Terminal.ConsoleApp.Views;
internal static class SingleUserReportView
{
    public static void Render(SingleUserReportResult? report)
    {
        if (report == null)
        {
            Console.WriteLine("<No data>");
            return;
        }

        // Console.WriteLine($"Report execution status={report.ExecutionStatus}");
        WriteUserInfo(report);

        // Account not changed yet
        // var accountsTable = CreateAccountsTable(report.Accounts);
        // AnsiConsole.Write(accountsTable);

        var ordersTable = CreateOrdersTable(report.Orders);
        AnsiConsole.Write(ordersTable);
        AnsiConsole.WriteLine("\n");

        var positionsTable = CreatePositionsTable(report.Positions);
        AnsiConsole.Write(positionsTable);
        AnsiConsole.WriteLine("\n");
    }

    private static void WriteUserInfo(SingleUserReportResult report)
    {
        var user = StaticContext.Users.All.GetById(report.Uid);

        var deviderTop = new string('=', 80);
        Console.WriteLine($"\n{deviderTop}");

        Console.WriteLine($"User: Id={report.Uid} Name={user!.Name}  Status={report.UserStatus}");

        var deviderBottom = new string('-', 80);
        Console.WriteLine($"{deviderBottom}\n");
    }

    private static Table CreateOrdersTable(IDictionary<int, OrderDto[]> orders)
    {
        var table = new Table
        {
            Title = new TableTitle("Open orders"),
        };

        table.AddColumns(
            "Symbol",
            "OrderId",
            "Timestamp",
            "Action",
            "Size",
            "Filled",
            "Remaining",
            "Price"
            );

        foreach (var kvp in orders)
        {
            var symbol = StaticContext.Symbols.All.GetById(kvp.Key);

            foreach (var order in kvp.Value.OrderBy(v => v.Action).ThenBy(v => v.Price))
            {
                table.AddRow(
                    symbol!.Code,
                    order.OrderId.ToString(),
                    order.Timestamp.ToString(ViewConsts.TimeFormat),
                    order.Action.ToString(),
                    order.Size.ToString(),
                    order.Filled.ToString(),
                    order.Remaining.ToString(),
                    order.Price.ToString(ViewConsts.DecimalFormat)
                    );
            }
        };

        return table;
    }
    private static Table CreatePositionsTable(IDictionary<int, PositionDto> positions)
    {
        var table = new Table
        {
            Title = new TableTitle("Positions"),
        };

        table.AddColumns(
            "Symbol",
            "Direction",
            // "PnL",
            "Fixed PnL",
            "Open Price Sum",
            "Open Size"
            );

        foreach (var kvp in positions)
        {
            var symbol = StaticContext.Symbols.All.GetById(kvp.Key);
            var pos = kvp.Value;

            table.AddRow(
                symbol!.Code,
                pos.Direction.ToString(),
                // pos.PnL.ToString(ViewConsts.DecimalFormat),
                pos.FixedPnL.ToString(ViewConsts.DecimalFormat),
                pos.OpenPriceSum.ToString(ViewConsts.DecimalFormat),
                pos.OpenVolume.ToString()
                );
        };

        return table;
    }

    /*
    private static Table CreateAccountsTable(IDictionary<int, decimal> accounts)
    {
        var table = new Table
        {
            Title = new TableTitle("Accounts"),
        };

        table.AddColumns("Currency", "Amount");

        foreach (var acc in accounts)
        {
            var currency = StaticContext.Currencies.All.GetById(acc.Key);
            table.AddRow(currency!.Code, acc.Value.ToString(ViewConsts.DecimalFormat));
        };

        return table;
    }
    */
}
