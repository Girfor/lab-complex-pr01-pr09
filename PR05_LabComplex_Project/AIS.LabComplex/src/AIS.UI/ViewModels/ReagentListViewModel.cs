using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AIS.UI.ViewModels;

public sealed record ReagentDto(
    Guid Id, string Name, string ExternalCode, string Lot,
    decimal Quantity, string Unit, DateOnly ExpirationDate);

public sealed record ReagentPage(List<ReagentDto> Items, int Total, int Page, int PageSize);

public sealed class RelayCommand : ICommand
{
    private readonly Action _execute;
    public RelayCommand(Action execute) => _execute = execute;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute();
    public event EventHandler? CanExecuteChanged { add { } remove { } }
}

public sealed class ReagentListViewModel : INotifyPropertyChanged
{
    // Только локальный демонстрационный ключ. В реальном UI используется OIDC.
    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:5080/")
    };
    private string _search = "";
    private string _status = "Подключение к API...";
    public ObservableCollection<ReagentDto> Reagents { get; } = new();
    public ICommand RefreshCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Search
    {
        get => _search;
        set { _search = value; OnPropertyChanged(); }
    }

    public string Status
    {
        get => _status;
        private set { _status = value; OnPropertyChanged(); }
    }

    public ReagentListViewModel()
    {
        _http.DefaultRequestHeaders.Add("X-Demo-Key", "local-research-example-2026");
        RefreshCommand = new RelayCommand(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        try
        {
            Status = "Загрузка...";
            var query = Uri.EscapeDataString(Search);
            var response = await _http.GetFromJsonAsync<ReagentPage>(
                $"api/v1/reagents?q={query}&page=1&pageSize=100");
            Reagents.Clear();
            foreach (var item in response?.Items ?? new List<ReagentDto>())
                Reagents.Add(item);
            Status = $"Найдено реактивов: {response?.Total ?? 0}";
        }
        catch (Exception e)
        {
            Status = "Ошибка API. Запустите AIS.API: " + e.Message;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? property = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
