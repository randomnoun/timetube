using System;
using System.Collections.Generic;
using System.Text;

/* borrowed from http://dotnetjunkies.com/WebLog/malio/archive/2004/10/04/27603.aspx */

namespace Timetube {

  /// <summary>
  /// reads icons from the windows shell
  /// </summary>
  public class IconReader {

    /// <summary>
    /// used to get/return information about a file
    /// in SHGetFileInfo call 
    /// </summary>
    [System.FlagsAttribute()]
    private enum EnumFileInfoFlags : uint {
      /// <summary>get large icon</summary>
      LARGEICON           = 0x000000000,      
      /// <summary>get small icon</summary>
      SMALLICON           = 0x000000001,      
      /// <summary>get open icon</summary>
      OPENICON            = 0x000000002,      
      /// <summary>get shell size icon</summary>
      SHELLICONSIZE       = 0x000000004,      
      /// <summary>pszPath is a pidl</summary>
      PIDL                = 0x000000008,      
      /// <summary>use passed dwFileAttribute</summary>
      USEFILEATTRIBUTES   = 0x000000010,      
      /// <summary>apply the appropriate overlays</summary>
      ADDOVERLAYS         = 0x000000020,      
      /// <summary>get the index of the overlay</summary>
      OVERLAYINDEX        = 0x000000040,      
      /// <summary>get icon</summary>
      ICON                = 0x000000100,      
      /// <summary>get display name</summary>
      DISPLAYNAME          = 0x000000200,      
      /// <summary>get type name</summary>
      TYPENAME            = 0x000000400,     
      /// <summary>get attributes</summary>
      ATTRIBUTES          = 0x000000800,      
      /// <summary>get icon location</summary>
      ICONLOCATION        = 0x000001000,      
      /// <summary>return exe type</summary>
      EXETYPE             = 0x000002000,      
      /// <summary>get system icon index</summary>
      SYSICONINDEX        = 0x000004000,      
      /// <summary>put a link overlay on icon</summary>
      LINKOVERLAY         = 0x000008000,      
      /// <summary>show icon in selected state</summary>
      SELECTED            = 0x000010000,      
      /// <summary>get only specified attributes</summary>
      ATTR_SPECIFIED      = 0x000020000       
    }

    /// <summary>
    /// maxumum length of path 
    /// </summary>
    private const int conMAX_PATH = 260;

    /// <summary>
    /// looking for folder
    /// </summary>
      private const uint conFILE_ATTRIBUTE_DIRECTORY = 0x00000010;  

    /// <summary>
    /// looking for file
    /// </summary>
      private const uint conFILE_ATTRIBUTE_NORMAL = 0x00000080;  

    /// <summary>
    /// size of the icon
    /// </summary>
      public enum EnumIconSize {
      /// <summary>32x32</summary>
      Large = 0,
      /// <summary>16x16</summary>
      Small = 1
    }
        
    /// <summary>
    /// state of the folder
    /// </summary>
    public enum EnumFolderType {
      /// <summary>open folder</summary>
      Open = 0,
      /// <summary>closed folder</summary>
      Closed = 1
    }

    /// <summary>
    /// hold file/icon information
    /// see platformSDK SHFILEINFO
    /// </summary>
    /// <remarks>
    /// be sure to call DestroyIcon [hIcon] when done
    /// </remarks>
    [System.Runtime.InteropServices.StructLayout(
       System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct ShellFileInfo { 

      public const int          conNameSize = 80;
      public System.IntPtr      hIcon;  // note to call DestroyIcon
      public int                iIndex; 
      public uint                dwAttributes; 

      [System.Runtime.InteropServices.MarshalAs(
         System.Runtime.InteropServices.UnmanagedType.ByValTStr, 
         SizeConst=conMAX_PATH)]
      public string              szDisplayName; 

      [System.Runtime.InteropServices.MarshalAs(
         System.Runtime.InteropServices.UnmanagedType.ByValTStr, 
         SizeConst=conNameSize)]
      public string              szTypeName; 
    };

    /// <summary>
    /// used to free a windows icon handle
    /// </summary>
    /// <param name="hIcon">icon handle.</param>
    [System.Runtime.InteropServices.DllImport("User32.dll")]
    private static extern int DestroyIcon(System.IntPtr hIcon);

    /// <summary>
    /// gets file information 
    /// see platformSDK
    /// </summary>
    [System.Runtime.InteropServices.DllImport("Shell32.dll")]
    private static extern System.IntPtr SHGetFileInfo(
      string pszPath,
      uint dwFileAttributes,
      ref ShellFileInfo psfi,
      uint cbFileInfo,
      uint uFlags
    );

    /// <summary>
    /// lookup and return an icon from windows shell
    /// </summary>
    /// <param name="name">path to the file</param>
    /// <param name="size">large or small</param>
    /// <param name="linkOverlay">true to include the overlay link iconlet</param>
    /// <returns>requested icon</returns>
    public static System.Drawing.Icon GetFileIcon(
      string filePath, 
      EnumIconSize size, 
      bool addLinkOverlay) 
    {

      EnumFileInfoFlags flags = 
        EnumFileInfoFlags.ICON | EnumFileInfoFlags.USEFILEATTRIBUTES;

      // add link overlay if requested
      if (addLinkOverlay) {
        flags |= EnumFileInfoFlags.LINKOVERLAY;
      }

      // set size
      if (size == EnumIconSize.Small) {
        flags |= EnumFileInfoFlags.SMALLICON;
      }
      else {
        flags |= EnumFileInfoFlags.LARGEICON;
      }

      ShellFileInfo shellFileInfo = new ShellFileInfo();

      SHGetFileInfo(
        filePath, 
        conFILE_ATTRIBUTE_NORMAL, 
        ref shellFileInfo, 
        (uint)System.Runtime.InteropServices.Marshal.SizeOf(shellFileInfo), 
        (uint)flags);

      // deep copy 
      System.Drawing.Icon icon = 
      (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shellFileInfo.hIcon).Clone();

      // release handle 
      DestroyIcon(shellFileInfo.hIcon);    

      return icon;
    }

    /// <summary>
    ///  lookup and return an icon from windows shell
    /// </summary>
    /// <param name="size">large or small</param>
    /// <param name="folderType">open or closed</param>
    /// <returns>requested icon</returns>
    public static System.Drawing.Icon GetFolderIcon(
      EnumIconSize size, 
      EnumFolderType folderType) 
    {
      return GetFolderIcon(null, size, folderType);
    }

    /// <summary>
    /// lookup and return an icon from windows shell
    /// </summary>
    /// <param name="folderPath">path to folder<param>    
    /// <param name="size">large or small</param>
    /// <param name="folderType">open or closed</param>
    /// <returns>requested icon</returns>
    public static System.Drawing.Icon GetFolderIcon(
      string folderPath, 
      EnumIconSize size, 
      EnumFolderType folderType) 
    {
      
      EnumFileInfoFlags flags = 
        EnumFileInfoFlags.ICON | EnumFileInfoFlags.USEFILEATTRIBUTES;

      if (folderType == EnumFolderType.Open) {
        flags |= EnumFileInfoFlags.OPENICON;
      }
      
      if (EnumIconSize.Small == size) {
        flags |= EnumFileInfoFlags.SMALLICON;
      } 
      else {
        flags |= EnumFileInfoFlags.LARGEICON;
      }
      
      ShellFileInfo shellFileInfo = new ShellFileInfo();
      SHGetFileInfo(
        folderPath, 
        conFILE_ATTRIBUTE_DIRECTORY, 
        ref shellFileInfo, 
        (uint)System.Runtime.InteropServices.Marshal.SizeOf(shellFileInfo), 
        (uint)flags );    

      // deep copy 
      System.Drawing.Icon icon = 
        (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shellFileInfo.hIcon).Clone();

      // release handle
      DestroyIcon(shellFileInfo.hIcon);  

      return icon;
    }  
  }

}
