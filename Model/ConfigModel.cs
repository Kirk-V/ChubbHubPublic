using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Model
{

    /*
     * An object to hold the configuration settings for the chubb all user report. This will
     * help to pull the appropriate departmental information for the Chubb report.
     */
    public class ConfigModel
    {
        //Hard coded default file locations.
        private string _defaultDepartmentCodeFile;
        private string _defaultAuthorityGroupsDile;

        private string? _departmentCodeFile;

        
        private string? _authorityGroupsFile;

        


        /*
         * Dictionary of the department codes and associated authoritygroup
         */
        private Dictionary<string, AuthorityGroup>? _authorityGroupDepartmentCodes;

        public ConfigModel()
        {

        }
    }
}
