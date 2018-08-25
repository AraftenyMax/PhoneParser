namespace PhoneParser.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PhoneModel : DbContext
    {
        public PhoneModel()
            : base("name=PhoneModel")
        {
        }

        public virtual DbSet<Phone> Phones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Phone>()
                .Property(e => e.ScreenDiag)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.ScreenResolution)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.MainCamera)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.ProcessorName)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.ProcessorCores)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.ProcessorSpeed)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.VideoProcName)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.RAM)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.InternalMem)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.IsMemExpanded)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.MaxInternalMem)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.OS)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.AccessTypes)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.SIMQuantity)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.NavAbility)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.BatteryCapacity)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.StandaloneSpecs)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.WirelessCharge)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.FastCharge)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.BodyMaterial)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.WaterProtection)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.Size)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.Weight)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.PhoneName)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.InStock)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.ShopName)
                .IsUnicode(false);

            modelBuilder.Entity<Phone>()
                .Property(e => e.FrontalCamera)
                .IsUnicode(false);
        }
    }
}
