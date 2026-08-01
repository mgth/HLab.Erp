using System;
using System.Threading.Tasks;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Data;

public static class DbDeleteExtention
{
    public static async Task<bool> DeleteModelAsync<T>(this IViewModel<T> viewModel, IDialogService navigationService, IDataService dbService, string deleteMessage=null, string caption=null)
    where T : class, IEntity
    {
        var e = viewModel.Model;
        try
        {
                using (var transaction = dbService.GetTransaction())
                {
                    dbService.Delete(e);
                    //db.SaveChanges();
                    if (string.IsNullOrEmpty(deleteMessage) ||
                        await navigationService.ShowMessageYesNoAsync(caption, deleteMessage, "Question"))

                        transaction.Done();
                    else
                    {
                        return false;
                    }
                }



            //dbService.Execute(db =>
            //{
            //    db.Remove(e);
            //    db.SaveChanges();
            //});
            viewModel.Model = null;
        }
        catch(Exception)
        {
            await navigationService.ShowMessageOkAsync("Suppression Impossible", caption ?? "", "Error");
            return false;
        }
        return true;
        //e.Delete();
    }

}
