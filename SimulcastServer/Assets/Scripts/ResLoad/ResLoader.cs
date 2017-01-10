using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ResLoader
{
    private string mConfigPath = "Config.xml";
    private static ResLoader instance = null;
    public static ResLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResLoader();
            }
            return instance;
        }
    }

    public List<MovieData> mDataList = new List<MovieData>();
    public List<MovieData> DataList
    {
        get { return mDataList; }
    }


    public void LoadConfig()
    {
        try
        {
            mConfigPath = Path.Combine(GetRootPath(), mConfigPath);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(mConfigPath);
            XmlNode root = xmldoc.SelectSingleNode("movies");

            foreach (XmlNode nd in root.ChildNodes)
            {
                XmlElement element = (XmlElement)nd;

                MovieData movie = new MovieData();
                if (element != null)
                {
                    movie.VideoName = element.GetAttribute("name");
                    movie.VideoTitle = element.GetAttribute("title");
                    movie.VideoPath = element.GetAttribute("vidopath");
                    movie.CoverPath = element.GetAttribute("coverpath");

                    mDataList.Add(movie);
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public string GetRootPath()
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..")) + "\\Data";
    }
}


public class MovieData
{
    public string VideoName { get; set; }
    public string VideoTitle { get; set; }
    public string VideoPath { get; set; }
    public string CoverPath { get; set; }

    public MovieData() { }

    public MovieData(string name, string title, string videopath, string coverpath)
    {
        this.VideoName = name;
        this.VideoTitle = title;
        this.VideoPath = videopath;
        this.CoverPath = coverpath;
    }
}