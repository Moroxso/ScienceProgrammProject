using ScienceProgrammProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScienceProgrammProject.Core.DataScripts
{
    public interface IDatabaseService
    {
        bool IsConnected { get; }
        ScienceProgrammProjectEntities Context { get; }
        void Initialize();
        void TestConnection();
        void CreateBackup();
        void RestoreFromBackup(string backupPath);
    }
}
