using GeoFieldsApi.Services;
using GeoFieldsApi.Services.GeoFields;

var builder = WebApplication.CreateBuilder(args);

/*
 � ��������� �������, ��� ����� DI, � ����� �� ����� � �������. ��� ����� ��������, ��� ��� ��������� ���������� ��������� Startup, 
 � ��� �� ����� �������� Program.cs �������� �������, � �������� �� � ��������� ���� � ������.

 � ������ � ��������, ����� � ������� ���� ������� ���� � ��������: BaseController, Repository, Service, Interface, Entity.

 ����� � ��������� �������, ��� ���������� ���� �� ���������� ����������� ���� ������ (DbContext), � ��� ����� � ������� �� � Docker, � ��������� DNS, DHCP, 
 � ��� ����� ���������� �������� ������, �������, ��������, CI/CD, ���������� � ������ best practices.

 ������ ����� �������, ��� �� ������ ������ ��� ���������� �����: ����������� ����������, ������� ������ � ���������� ����� ������. 
 */

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKmlService, KmlService>();
builder.Services.AddSingleton<IGeoCalculationService, GeoCalculationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var kmlService = scope.ServiceProvider.GetRequiredService<IKmlService>();
    await kmlService.InitializeAsync();
}

app.Run();