using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BMEDmgt.Areas.MedEngMgt.Models;

namespace BMEDmgt.Models
{
    public class BMEDcontext: DbContext
    {
        public BMEDcontext()
        :base("AzureConnection") { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<CustOrgan> CustOrgans { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AppRoles> AppRoles { get; set; }
        public DbSet<UsersInRoles> UsersInRoles { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorType> VendorTypes { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<RepairDtl> RepairDtls { get; set; }
        public DbSet<RepairDtlRecord> RepairDtlRecords { get; set; }
        public DbSet<RepairFlow> RepairFlows { get; set; }
        public DbSet<RepairCost> RepairCosts { get; set; }
        public DbSet<Keep> Keeps { get; set; }
        public DbSet<KeepDtl> KeepDtls { get; set; }
        public DbSet<KeepEmp> KeepEmps { get; set; }
        public DbSet<KeepFlow> KeepFlows { get; set; }
        public DbSet<KeepCost> KeepCosts { get; set; }
        public DbSet<KeepRecord> KeepRecords { get; set; }
        public DbSet<KeepFormat> KeepFormats { get; set; }
        public DbSet<KeepFormatDtl> KeepFormatDtls { get; set; }
        public DbSet<DeptStok> DeptStoks { get; set; }
        public DbSet<StokRecord> StokRecords { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketDtl> TicketDtls { get; set; }
        public DbSet<Ticket_seq_tmp> Ticket_seq_tmps { get; set; }
        public DbSet<RepairEmp> RepairEmps { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetKeep> AssetKeeps { get; set; }
        public DbSet<AttainFile> AttainFiles { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Manual> Manuals { get; set; }
        public DbSet<DealStatus> DealStatus { get; set; }
        public DbSet<KeepResult> KeepResults { get; set; }
        public DbSet<FailFactor> FailFactors { get; set; }
        public DbSet<DeviceClassCode> DeviceClassCodes { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<QuestionnaireM> QuestionnaireMs { get; set; }

        public DbSet<QuestMain> QuestMains { get; set; }

        public DbSet<QuestAnswer> QuestAnswers { get; set; }

        public DbSet<QuestSend> QuestSends { get; set; }

        public DbSet<Func> Funcs { get; set; }
        public DbSet<FuncsInRoles> FuncsInRoles { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        public DbSet<AssetPurchaseContract> AssetPurchaseContracts { get; set; }
        public DbSet<AssetMaintainContract> AssetMaintainContracts { get; set; }
        public DbSet<NoAssetNoList> NoAssetNoLists { get; set; }
        public DbSet<EngSubStaff> EngSubStaffs { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DelivFlow> DelivFlows { get; set; }
        public DbSet<AssetPContractPermit> AssetPContractPermits { get; set; }
        public DbSet<AssetsInMContracts> AssetsInMContracts { get; set; }
        public DbSet<NeedFile> NeedFiles { get; set; }
        public DbSet<AssetFile> AssetFiles { get; set; }
        public DbSet<DelivCode> DelivCodes { get; set; }
    }
}