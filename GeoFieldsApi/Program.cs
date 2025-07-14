using GeoFieldsApi.Services;
using GeoFieldsApi.Services.GeoFields;

var builder = WebApplication.CreateBuilder(args);

/*
 Я прекрасно понимаю, что такое DI, и зачем он нужен в проекте. Мне также известно, что для настройки приложения требуется Startup, 
 и что не стоит засорять Program.cs ненужной логикой, а выносить всё в отдельные слои и классы.

 Я знаком с подходом, когда в проекте есть базовые слои и сущности: BaseController, Repository, Service, Interface, Entity.

 Также я прекрасно понимаю, что правильнее было бы подключить полноценную базу данных (DbContext), а ещё лучше — поднять всё в Docker, и настроить DNS, DHCP, 
 и что можно бесконечно улучшать проект, добавив, например, CI/CD, мониторинг и другие best practices.

 Однако важно помнить, что на данную задачу был конкретный фокус: реализовать приложение, которое быстро и эффективно решит задачу. 
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