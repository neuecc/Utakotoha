using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Moles.Framework;

// アセンブリに関する一般情報は以下の属性セットを通して制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Utakotoha.Test")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Utakotoha.Test")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから
// 参照できなくなります。このアセンブリ内で COM から型にアクセスする必要がある場合は、 
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID がタイプ ライブラリの ID になります。
[assembly: Guid("adfae4dd-eb7b-48ea-96ed-c4e8ca013e28")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// すべての値を指定するか、以下のように '*' を使用してビルド番号とリビジョン番号を 
// 既定値にすることができます:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Moles allow types
[assembly: MoledType(typeof(System.IO.IsolatedStorage.IsolatedStorageFile))]