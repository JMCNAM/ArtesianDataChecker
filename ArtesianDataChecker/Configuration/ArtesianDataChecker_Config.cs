using System;
using System.Collections.Generic;
using System.Text;
using ArtesianDataChecker.ClassObjects;

namespace ArtesianDataChecker.Configuration
{
    class ArtesianDataChecker_Config
    {
        public Config GetConfig(string provider)
        {
            Config con = new Config();
            if (provider == "Test")
            {
                con.baseAddress = "https://arktest-arkive-service-mhqvdrmdxmeza.azurewebsites.net/";
                con.audiance = "https://arkive.artesian.cloud/ArkTest/";
                con.domain = "arklab.eu.auth0.com";
                con.clientId = "MBlBDBCWO0z9nwl2b6N3uPb4CsJcEcLb";
                con.clientSec = "Gdd5ls5b3LpY8TMvlnC3VtyIAL2i-VmydE69_GchvagJNcfBjEhY0tx8IlJT3Dt0";
            }
            else if (provider == "ProdDemo")
            {
                con.baseAddress = "https://arkdemo-arkive-service-lv2sbihwhix2i.azurewebsites.net";
                con.audiance = "https://arkive.artesian.cloud/ArkDemo/";
                con.domain = "arklab.eu.auth0.com";
                con.clientId = "Xdw9kApoHTgPQU3C2SibQDQ5EzHxTFCZ";
                con.clientSec = "mVky9s1bNvsmDAGF3Uo35ZeIpevXTgv4slfY8Ul1THIxcucuJg38FBsqfh1anL-p";
            }
            else if (provider == "ProdDufe")
            {
                con.baseAddress = "https://arkive-dufe.azurewebsites.net";
                con.audiance = "https://arkive.artesian.cloud/DufeIT/";
                con.domain = "artesian.eu.auth0.com";
                con.clientId = "ZxUhM1lj8nsglDhgKnlP6V4FkIq2RsDR";
                con.clientSec = "6i-95Kx2-cQHBmG1cbGGJOoizqI9DkN7JKMSAMUpMGLsdu4JmhiCNGAgVfTXIf2W";
            }
            else
            {

            }
            return con;
        }
    }
}
