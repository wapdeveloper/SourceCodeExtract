using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;

namespace 源代码提取
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        string spath;
        string dpath;

        void GetAllPath(DirectoryInfo dir, TreeViewItem treeViewItem)
        {
            DirectoryInfo[] folders = dir.GetDirectories();

            if (dir is DirectoryInfo)
            {
                foreach (var item in folders)
                {
                    TreeViewItem treeViewfolder = new TreeViewItem(); //保存文件夹的Treeview
                    treeViewfolder.Header = item.Name;

                    if (item is DirectoryInfo)
                    {
                        treeViewItem.Items.Add(treeViewfolder);
                        GetAllPath(item, treeViewfolder);
                    }
                    else
                    {
                        TreeViewItem treeViewFileRoot = new TreeViewItem(); //保存文件的Treeview
                        treeViewFileRoot.Header = item.Name;
                        treeViewItem.Items.Add(treeViewFileRoot);
                        foreach (FileInfo file in item.GetFiles()) //获取每个文件夹下的文件
                        {
                            TreeViewItem treeViewfile = new TreeViewItem();
                            treeViewfile.Header = file.Name;
                            treeViewFileRoot.Items.Add(treeViewfile);
                        }
                    }
                }
                foreach (FileInfo file in dir.GetFiles()) //获取根目录的文件
                {
                    TreeViewItem treeViewfile = new TreeViewItem();
                    treeViewfile.Header = file.Name;
                    treeViewItem.Items.Add(treeViewfile);
                }
            }
        }

        private void btnin_Click(object sender, RoutedEventArgs e)
        {
            treeview1.Items.Clear();
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
                spath = fbd.SelectedPath;
            if (spath == null) return;
            string[] folder = Regex.Split(spath, "\\\\", RegexOptions.IgnoreCase);
            string foldername = folder[folder.Length - 1];

            DirectoryInfo dir = new DirectoryInfo(spath);
            DirectoryInfo[] fds = dir.GetDirectories();

            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = foldername;

            GetAllPath(dir, treeViewItem);
            this.treeview1.Items.Add(treeViewItem);

        }

        private void btnout_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
                dpath = fbd.SelectedPath;
            if (dpath == null) return;
            DirectoryInfo dir = new DirectoryInfo(dpath);
            DirectoryInfo[] fds = dir.GetDirectories();
            FileInfo[] fls = dir.GetFiles();
            if (fls.Length != 0 || fds.Length != 0)
                MessageBox.Show("请选择一个空文件夹！");
            else
            {
                string extensionName = txt1.Text;
                string[] names = extensionName.Split(',');
                if (txt1.Text == "")
                {
                    CopyDirectory(spath, dpath);
                }
                else
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        CopyExtensionDirectory(spath, dpath, names[i]);
                    }
                }
                MessageBox.Show("提取成功！");
            }
        }

        bool IsCSharpeFile(string name)
        {
            if (name.Substring(name.Length - 3, 3) == ".cs")
            {
                return true;
            }
            return false;
        }

        private bool IsCSharpeSoulution(string name)
        {
            if (name != "bin" && name != "obj" && name != "Debug" && name != "Release")
            {
                return true;
            }
            return false;
        }

        bool IsCPlusPlusFile(string name)
        {
            if (name.Substring(name.Length - 4, 4) == ".cpp" || name.Substring(name.Length - 2, 2) == ".c" || name.Substring(name.Length - 2, 2) == ".h")
            {
                return true;
            }
            return false;
        }

        bool IsCPlusPlusSoulution(string name)
        {
            if (name != "bin" && name != "obj" && name != "Debug" && name != "Release" && name != "lib" && name != "ipch")
            {
                if (name.Length > 5 && name.Substring(name.Length - 5, 5) != "_Debug")
                    return true;
            }
            return false;
        }


        bool IsFileExtension(string name, string extension)
        {
            if (name.Substring(name.Length - extension.Length, extension.Length) == extension)
            {
                return true;
            }
            return false;
        }

        public void CopyExtensionDirectory(string srcPath, string desPath, string extension)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo item in fileinfo)
                {
                    if (item is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(desPath + "\\" + item.Name))
                        {
                            Directory.CreateDirectory(desPath + "\\" + item.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyExtensionDirectory(item.FullName, desPath + "\\" + item.Name,extension);    //递归调用复制子文件夹
                    }
                    else
                    {
                        if (IsFileExtension(item.Name, extension))
                            File.Copy(item.FullName, desPath + "\\" + item.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public void CopyDirectory(string srcPath, string desPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo item in fileinfo)
                {
                    if (item is DirectoryInfo)     //判断是否文件夹
                    {
                        int index = cob1.SelectedIndex;
                        switch (index)
                        {
                            case 0:
                                {
                                    if (!Directory.Exists(desPath + "\\" + item.Name))
                                    {
                                        Directory.CreateDirectory(desPath + "\\" + item.Name);   //目标目录下不存在此文件夹即创建子文件夹
                                    }
                                    CopyDirectory(item.FullName, desPath + "\\" + item.Name);    //递归调用复制子文件夹
                                }
                                break;
                            case 1:
                                {
                                    if (IsCSharpeSoulution(item.Name))
                                    {
                                        if (!Directory.Exists(desPath + "\\" + item.Name))
                                        {
                                            Directory.CreateDirectory(desPath + "\\" + item.Name);   //目标目录下不存在此文件夹即创建子文件夹
                                        }
                                        CopyDirectory(item.FullName, desPath + "\\" + item.Name);    //递归调用复制子文件夹
                                    }
                                }
                                break;
                            case 2:
                                {
                                    if (!Directory.Exists(desPath + "\\" + item.Name))
                                    {
                                        Directory.CreateDirectory(desPath + "\\" + item.Name);   //目标目录下不存在此文件夹即创建子文件夹
                                    }
                                    CopyDirectory(item.FullName, desPath + "\\" + item.Name);    //递归调用复制子文件夹
                                }

                                break;
                            case 3:
                                {
                                    if (IsCPlusPlusSoulution(item.Name))
                                    {
                                        if (!Directory.Exists(desPath + "\\" + item.Name))
                                        {
                                            Directory.CreateDirectory(desPath + "\\" + item.Name);   //目标目录下不存在此文件夹即创建子文件夹
                                        }
                                        CopyDirectory(item.FullName, desPath + "\\" + item.Name);    //递归调用复制子文件夹
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                    }
                    else
                    {
                        int index = cob1.SelectedIndex;
                        switch (index)
                        {
                            case 0:
                                {
                                    if (IsCSharpeFile(item.Name))
                                        File.Copy(item.FullName, desPath + "\\" + item.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                                }
                                break;
                            case 1:
                                {
                                    File.Copy(item.FullName, desPath + "\\" + item.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                                }
                                break;
                            case 2:
                                {
                                    if (IsCPlusPlusFile(item.Name))
                                        File.Copy(item.FullName, desPath + "\\" + item.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                                }
                                break;
                            case 3:
                                {
                                    File.Copy(item.FullName, desPath + "\\" + item.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
