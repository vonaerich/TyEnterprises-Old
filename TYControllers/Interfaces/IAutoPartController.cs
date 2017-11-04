using System;
using System.Collections.Generic;
using TY.SPIMS.Entities;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IAutoPartController
    {
        bool AutoPartHasDetails(int id);
        bool AutoPartHasSalesOrPurchases(int id);
        void DeleteAllAutoPart(int id);
        void DeleteAutoPart(int id);
        List<AutoPartDetail> FetchAllAutoPartDetails();
        List<AutoPart> FetchAllAutoParts();
        List<string> FetchAllPartNames();
        string[] FetchAllPartNumbers();
        List<string> FetchAllPartNumbersWithAlternateNumbers();
        List<AutoPart> FetchAPWithSearch(string search);
        AutoPart FetchAutoPartById(int id);
        AutoPart FetchAutoPartByName(string name);
        AutoPartDetail FetchAutoPartDetailById(int id);
        AutoPartDetail FetchAutoPartDetailByPartNumber(string partNumber);
        List<AutoPartDetail> FetchAutoPartDetails(int id);
        List<AutoPartDetail> FetchAutoPartDetails(string partNumber);
        SortableBindingList<AutoPartDisplayModel> FetchAutoPartWithSearch(AutoPartFilterModel filter);
        string FetchPartNameById(int id);
        string FetchPartNameByPartNumber(string partNumber);
        void InsertAutoPart(AutoPartColumnModel model);
        void UpdateAutoPart(AutoPartColumnModel model);
        void UpdateAutoPartName(int id, string newAutoPart);
        void UpdateBuyPrice(int id, decimal newBuyPrice);
        void UpdatePartPrices(int id, decimal newBuyPrice, decimal newSell1, decimal newSell2);
        void UpdateReorderDates(List<int> ids);
    }
}
