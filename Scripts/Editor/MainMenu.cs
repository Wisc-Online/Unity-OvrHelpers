using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Editor
{
    public class Menu
    {
        const string OculusDesktopPackageId = "com.unity.xr.oculus.standalone";
        const string OculusIntegrationAssetStoreUrl = "https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022";

        private static AddRequest _addOculusToolsAssetRequest;

        [MenuItem("Learning Innovations/Ovr/Install Oculus Desktop Package", priority = 2)]
        public static void InstallOculusDesktopPackage()
        {
            _addOculusToolsAssetRequest = Client.Add(OculusDesktopPackageId);

            EditorApplication.update += OculusDesktopPackageInstallProgress;
        }

        private static void OculusDesktopPackageInstallProgress()
        {
            if (_addOculusToolsAssetRequest != null)
            {
                if (_addOculusToolsAssetRequest.IsCompleted)
                {
                    EditorApplication.update -= OculusDesktopPackageInstallProgress;

                    switch (_addOculusToolsAssetRequest.Status)
                    {
                        case StatusCode.Success:
                            EditorUtility.DisplayDialog("Package Installed", "Oculus Desktop Package Installed Successfully", "Yay!");
                            Debug.Log($"Oculus Tools Asset installed successfully");
                            break;

                        case StatusCode.Failure:
                        default:
                            EditorUtility.DisplayDialog("Package Installation Failed", "Oculus Desktop package failed to install.", "Close");
                            Debug.LogError($"Error installing Oculus Tools\n{_addOculusToolsAssetRequest.Error.message}");
                            break;
                    }
                }
            }
        }

        [MenuItem("Learning Innovations/Ovr/Install Oculus Integration Assets", priority = 1)]
        public static void InstallOculusIntegrationAssets()
        {
            bool doIt = EditorUtility.DisplayDialog("Install Oculus Integration", @"Press OK to open the Unity Asset Store in a browser.

From there, select either 'Add to My Assets' or 'Open in Unity'", "OK", "Cancel");

            if (doIt)
            {
                Application.OpenURL(OculusIntegrationAssetStoreUrl);
            }
        }

        [MenuItem("Learning Innovations/Ovr/Install Oculus Integration Assets", true)]
        public static bool InstallOculusIntegrationAssetsValidate()
        {
            bool isValid = true;

            isValid = AssetDatabase.IsValidFolder("Oculus");

            return isValid;
        }

    }
}
