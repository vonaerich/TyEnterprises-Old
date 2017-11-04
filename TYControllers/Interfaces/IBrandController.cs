using System.Collections.Generic;
using TY.SPIMS.Entities;
using TY.SPIMS.POCOs;
using TY.SPIMS.Utilities;

namespace TY.SPIMS.Controllers.Interfaces
{
    public interface IBrandController
    {
        void DeleteBrand(int id);
        List<Brand> FetchAllBrands();
        Brand FetchBrandById(int id);
        SortableBindingList<BrandDisplayModel> FetchBrandWithSearch(string filter);
        void InsertBrand(BrandColumnModel model);
        void UpdateBrand(BrandColumnModel model);
    }
}
