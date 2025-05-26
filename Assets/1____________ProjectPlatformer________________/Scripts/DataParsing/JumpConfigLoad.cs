using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpConfigLoad : MonoBehaviour
{
    public static Dictionary<string, JumpConfig> configDic = new Dictionary<string, JumpConfig>();

    private void Awake()
    {
        LoadConfig();
    }

    void LoadConfig()
    {
        // Resources ���� �ȿ� �ִ� jump_config.csv �ε�
        TextAsset csvFile = Resources.Load<TextAsset>("jump_config");

        // ������ �� ������ ������ �迭�� ����
        string[] lines = csvFile.text.Split('\n');

        // ù ���� ���(Header) �κ��̶� 1�� �ε������� ����
        for(int i = 1; i < lines.Length; i++)
        {
            // ���� ����ִٸ� ��ŵ
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            // �޸��� ������ (�� ������ �и�)
            string[] row = lines[i].Split(',');

            // �� ���� ������ ��� ��ŵ
            if (row.Length < 5) continue;

            // �� JumpConfig ��ü ����
            JumpConfig config = new JumpConfig
            {
                Name = row[1],          // �ι�° �� : name
                Type = row[2]           // ����° �� : Type
            };

            // Type �� ���� ForceValue �Ǵ� Multiplier �� �Ľ�
            if(config.Type == "Player")
            {
                float.TryParse(row[3], out config.ForceValue);      // �׹�° ��
            }
            if(config.Type == "JumpPlate")
            {
                float.TryParse(row[4], out config.Multiplier);      // �ټ���° ��
            }
            if (config.Type == "AirJump")
            {
                float.TryParse(row[3], out config.ForceValue);
                float.TryParse(row[4], out config.Multiplier);
            }

            // Dictionary �� ���� (�ߺ� Name �� �����)
            configDic[config.Name] = config;
        }
    }
}
