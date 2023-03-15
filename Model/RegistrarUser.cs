using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Model
{
    public class RegistrarUser : ChubbUser
    {
        private string? _acadGroup;
        public string AcadGroup
        {
            get => this._acadGroup ?? string.Empty;
            set
            {
                this._acadGroup = value;
                base.NotifyPropertyChanged(nameof(AcadGroup));
            }
        }

        private string? _primaryProgram;

        public string? PrimaryProgram
        {
            get => this._primaryProgram ?? string.Empty;
            set
            {
                this._primaryProgram = value;
                base.NotifyPropertyChanged(nameof(PrimaryProgram));
            }
        }

        private string? _startLevel;

        public string? StartLevel
        {
            get => this._startLevel ?? string.Empty;
            set { this._startLevel = value; base.NotifyPropertyChanged(nameof(StartLevel)); }
        }
        public RegistrarUser(string firstName, string lastName, string westernID) : base(firstName, lastName, westernID)
        {

        }

        public RegistrarUser(string firstName, string lastName, string westernID, string primProg, string startLevel, string acadGroup): base(firstName, lastName, westernID)
        {
            AcadGroup = acadGroup;
            PrimaryProgram = primProg;
            StartLevel = startLevel;
        }

        public bool UpdateMatchedUser(ChubbUser existingUser)
        {
            if(this.WesternId == existingUser.WesternId)
            {
                base.AuthorityNumber = existingUser.AuthorityNumber;
                base.AuthorityPlusNumber = existingUser.AuthorityPlusNumber;
                base.ChubbNumber = existingUser.ChubbNumber;
                base.ChubbStatus = existingUser.ChubbStatus;
                base.DeptCode = existingUser.DeptCode;
                base.Type = existingUser.Type;
                base.UserName = existingUser.UserName;
                base.Pin = existingUser.Pin;
                base.StatusCode = existingUser.StatusCode;
                base.SystemAuthority = existingUser.SystemAuthority;
                base.SystemAuthorityPlus = existingUser.SystemAuthorityPlus;
                base.ExpiryString = existingUser.ExpiryString;
                base.Expiry = existingUser.Expiry;
                base.ExpiryPlus = existingUser.ExpiryPlus;
                base.ExpiryPlusString = existingUser.ExpiryPlusString;
                return true;
            }
            return false;
        }

    }
}
