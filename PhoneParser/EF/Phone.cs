namespace PhoneParser.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Phone
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string ScreenDiag { get; set; }

        [StringLength(50)]
        public string ScreenResolution { get; set; }

        [StringLength(50)]
        public string MainCamera { get; set; }

        [StringLength(50)]
        public string ProcessorName { get; set; }

        [StringLength(50)]
        public string ProcessorCores { get; set; }

        [StringLength(50)]
        public string ProcessorSpeed { get; set; }

        [StringLength(50)]
        public string VideoProcName { get; set; }

        [StringLength(50)]
        public string RAM { get; set; }

        [StringLength(50)]
        public string InternalMem { get; set; }

        [StringLength(50)]
        public string IsMemExpanded { get; set; }

        [StringLength(50)]
        public string MaxInternalMem { get; set; }

        [StringLength(50)]
        public string OS { get; set; }

        [StringLength(50)]
        public string AccessTypes { get; set; }

        [StringLength(50)]
        public string SIMQuantity { get; set; }

        [StringLength(50)]
        public string NavAbility { get; set; }

        [StringLength(50)]
        public string BatteryCapacity { get; set; }

        [StringLength(1024)]
        public string StandaloneSpecs { get; set; }

        [StringLength(50)]
        public string WirelessCharge { get; set; }

        [StringLength(50)]
        public string FastCharge { get; set; }

        [StringLength(50)]
        public string BodyMaterial { get; set; }

        [StringLength(50)]
        public string WaterProtection { get; set; }

        [StringLength(50)]
        public string Size { get; set; }

        [StringLength(50)]
        public string Weight { get; set; }

        [StringLength(256)]
        public string PhoneName { get; set; }

        [StringLength(50)]
        public string InStock { get; set; }

        [StringLength(50)]
        public string ShopName { get; set; }

        [StringLength(50)]
        public string FrontalCamera { get; set; }

        [StringLength(256)]
        public string Url { get; set; }

        [StringLength(50)]
        public string Price { get; set; }
    }
}
