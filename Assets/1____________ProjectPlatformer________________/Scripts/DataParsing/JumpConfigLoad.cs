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
        // Resources 파일 안에 있는 jump_config.csv 로드
        TextAsset csvFile = Resources.Load<TextAsset>("jump_config");

        // 파일을 줄 단위로 나눠서 배열에 저장
        string[] lines = csvFile.text.Split('\n');

        // 첫 줄은 헤더(Header) 부분이라 1번 인덱스부터 시작
        for(int i = 1; i < lines.Length; i++)
        {
            // 줄이 비어있다면 스킵
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            // 콤마로 나누기 (열 단위로 분리)
            string[] row = lines[i].Split(',');

            // 열 수가 부족한 경우 스킵
            if (row.Length < 5) continue;

            // 새 JumpConfig 객체 생성
            JumpConfig config = new JumpConfig
            {
                Name = row[1],          // 두번째 열 : name
                Type = row[2]           // 세번째 열 : Type
            };

            // Type 에 따라 ForceValue 또는 Multiplier 만 파싱
            if(config.Type == "Player")
            {
                float.TryParse(row[3], out config.ForceValue);      // 네번째 열
            }
            if(config.Type == "JumpPlate")
            {
                float.TryParse(row[4], out config.Multiplier);      // 다섯번째 열
            }
            if (config.Type == "AirJump")
            {
                float.TryParse(row[3], out config.ForceValue);
                float.TryParse(row[4], out config.Multiplier);
            }

            // Dictionary 에 저장 (중복 Name 시 덮어쓰기)
            configDic[config.Name] = config;
        }
    }
}
