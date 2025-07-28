namespace CVMatcherApp.Api.Services.Base;
public interface ICVService
{
    Task<int> CleanUpOldCVs();    
}