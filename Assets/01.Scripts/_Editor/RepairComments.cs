using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
public class Repair
{
    private static readonly string[] k_Exts = { ".cs", ".shader", ".json", ".txt" };
   
    [MenuItem("Tools/Encoding/Convert Scripts Folder (UTF-8 BOM)")]
    private static void Convert() {

        string root = Path.Combine(UnityEngine.Application.dataPath, "01.Scripts");

        int addedBom = 0, convertedCp = 0, skipped = 0, error = 0;

        if (!Directory.Exists(root)) {
            EditorUtility.DisplayDialog("오류", $"경로가 없습니다:\n{root}", "확인");
            return;
        }

        foreach (string file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)) {
            if (!Array.Exists(k_Exts, e => file.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
                continue;

            byte[] raw = File.ReadAllBytes(file);
            if (HasBom(raw)) { skipped++; continue; }

            try {
                if (IsStrictUtf8(raw)) {
                    // UTF‑8 (무 BOM) → BOM 추가
                    File.WriteAllText(file, Encoding.UTF8.GetString(raw), new UTF8Encoding(true));
                    addedBom++;
                } else {
                    // CP949 (ANSI) → UTF‑8 (BOM)
                    File.WriteAllText(file, Encoding.GetEncoding(949).GetString(raw), new UTF8Encoding(true));
                    convertedCp++;
                }
            } catch (Exception ex) {
                Debug.LogError($"[Encoding] {file} : {ex.Message}");
                error++;
            }
        }

        EditorUtility.DisplayDialog(
            "Scripts 폴더 인코딩 결과",
            $"BOM 추가        : {addedBom}\n" +
            $"CP949 변환   : {convertedCp}\n" +
            $"스킵(이미 BOM) : {skipped}\n" +
            $"오류              : {error}",
            "확인");

        AssetDatabase.Refresh();
    }

    private static bool HasBom(byte[] b) =>
        b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF;

    private static bool IsStrictUtf8(byte[] b) {
        try {
            new UTF8Encoding(false, true).GetString(b); // 잘못된 UTF‑8이면 예외
            return true;
        } catch { return false; }
    }
}
