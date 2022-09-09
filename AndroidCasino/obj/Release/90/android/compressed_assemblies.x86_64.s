	.file	"compressed_assemblies.x86_64.x86_64.s"
	.include	"compressed_assemblies.x86_64-data.inc"

	.section	.data.compressed_assembly_descriptors,"aw",@progbits
	.type	.L.compressed_assembly_descriptors, @object
	.p2align	4
.L.compressed_assembly_descriptors:
	/* 0: AndroidCasino.dll */
	/* uncompressed_file_size */
	.long	6656
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_0

	/* 1: CasinoSharedProject.dll */
	/* uncompressed_file_size */
	.long	149504
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_1

	/* 2: Classes.dll */
	/* uncompressed_file_size */
	.long	56832
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_2

	/* 3: Java.Interop.dll */
	/* uncompressed_file_size */
	.long	162304
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_3

	/* 4: Mono.Android.dll */
	/* uncompressed_file_size */
	.long	1073152
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_4

	/* 5: Mono.Security.dll */
	/* uncompressed_file_size */
	.long	121856
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_5

	/* 6: MonoGame.Extended.Gui.dll */
	/* uncompressed_file_size */
	.long	75776
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_6

	/* 7: MonoGame.Extended.Input.dll */
	/* uncompressed_file_size */
	.long	38400
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_7

	/* 8: MonoGame.Extended.dll */
	/* uncompressed_file_size */
	.long	158208
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_8

	/* 9: MonoGame.Framework.dll */
	/* uncompressed_file_size */
	.long	1079296
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_9

	/* 10: Newtonsoft.Json.dll */
	/* uncompressed_file_size */
	.long	684544
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_10

	/* 11: Server.dll */
	/* uncompressed_file_size */
	.long	27648
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_11

	/* 12: System.ComponentModel.DataAnnotations.dll */
	/* uncompressed_file_size */
	.long	5632
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_12

	/* 13: System.Core.dll */
	/* uncompressed_file_size */
	.long	413184
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_13

	/* 14: System.Data.dll */
	/* uncompressed_file_size */
	.long	1281536
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_14

	/* 15: System.Drawing.Common.dll */
	/* uncompressed_file_size */
	.long	9216
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_15

	/* 16: System.Net.Http.dll */
	/* uncompressed_file_size */
	.long	220160
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_16

	/* 17: System.Numerics.dll */
	/* uncompressed_file_size */
	.long	38912
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_17

	/* 18: System.Runtime.Serialization.dll */
	/* uncompressed_file_size */
	.long	6144
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_18

	/* 19: System.Transactions.dll */
	/* uncompressed_file_size */
	.long	10752
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_19

	/* 20: System.Xml.Linq.dll */
	/* uncompressed_file_size */
	.long	65024
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_20

	/* 21: System.Xml.dll */
	/* uncompressed_file_size */
	.long	1369600
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_21

	/* 22: System.dll */
	/* uncompressed_file_size */
	.long	875520
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_22

	/* 23: Usefull Methods.dll */
	/* uncompressed_file_size */
	.long	5632
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_23

	/* 24: mscorlib.dll */
	/* uncompressed_file_size */
	.long	2087424
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_24

	.size	.L.compressed_assembly_descriptors, 400
	.section	.data.compressed_assemblies,"aw",@progbits
	.type	compressed_assemblies, @object
	.p2align	3
	.global	compressed_assemblies
compressed_assemblies:
	/* count */
	.long	25
	/* descriptors */
	.zero	4
	.quad	.L.compressed_assembly_descriptors
	.size	compressed_assemblies, 16
