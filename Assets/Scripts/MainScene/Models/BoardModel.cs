using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardModel : MonoBehaviour
{
    [Header("Parameters")]
    // コマを上の世界においたときの高さ
    [SerializeField] float upperWorldHeight = 1.71f;
    // コマを下の世界においたときの高さ
    [SerializeField] float downerWorldHeight = 1.4f;

    // マス目のワールド座標
    float[,] worldPoints = new float[25, 2]{
        // 上の世界の{x,z}座標
        {0.0f, 0.0f}, {3.2f, 0.0f}, {6.4f, 0.0f}, {9.6f, 0.0f},
        {0.0f, 3.2f}, {3.2f, 3.2f}, {6.4f, 3.2f}, {9.6f, 3.2f},
        {0.0f, 6.4f}, {3.2f, 6.4f}, {6.4f, 6.4f}, {9.6f, 6.4f},
        {0.0f, 9.6f}, {3.2f, 9.6f}, {6.4f, 9.6f}, {9.6f, 9.6f},
        // 下の世界の{x,z}座標
        {1.6f, 1.6f}, {4.8f, 1.6f}, {8.0f, 1.6f},
        {1.6f, 4.8f}, {4.8f, 4.8f}, {8.0f, 4.8f},
        {1.6f, 8.0f}, {4.8f, 8.0f}, {8.0f, 8.0f},
    };
    // 読み込み用変数
    public float[,] WP{ get { return worldPoints; } }
    // 1ターン目に置けるマス
    int[] startPoints = new int[] { 5, 6, 9, 10 };
    // 8ターン目以降に移動できる隣接マス
    int[][] neighborhood = new int[][] {
        // 上の世界と隣接する下の世界の座標番号
        new int[]{16}, new int[]{16,17}, new int[]{17,18}, new int[]{18},
        new int[]{16,19}, new int[]{16,17,19,20}, new int[]{17,18,20,21}, new int[]{18,21},
        new int[]{19,22}, new int[]{19,20,22,23}, new int[]{20,21,23,24}, new int[]{21,24},
        new int[]{22}, new int[]{22,23}, new int[]{23,24}, new int[]{24},

        // 下の世界と隣接する上の世界の座標番号
        new int[]{0,1,4,5}, new int[]{1,2,5,6}, new int[]{2,3,6,7},
        new int[]{4,5,8,9}, new int[]{5,6,9,10}, new int[]{6,7,10,11},
        new int[]{8,9,12,13}, new int[]{9,10,13,14}, new int[]{10,11,14,15},
    };

    // 青:1/赤:-1/空:0 でマスのスコアを管理する
    int[] scoreArray = new int[25];
    public int[] ScoreArray { get { return scoreArray; } }

    public void Initialize()
    {
        for(int i = 0;i < scoreArray.Length; i++)
        {
            scoreArray[i] = 0;
        }
    }
    
    // ======================================
    // ボードの状態を管理する関数
    // ======================================
    // 成功したら指定したマスの座標を，失敗したら-100を返す
    public Vector3 PutSimpei(int point, int oldPoint,int turnNum, int simpeiColor)
    {
        Vector3 target = new Vector3(-100, -100, -100);
        if (turnNum == 1)
        {
            // 1ターン目にスタートマス以外なら失敗
            if (!IsInStartPoint(point)) return target;
        }
        if (turnNum > 8)
        {
            // 8ターン目以降は隣接マス以外なら失敗
            if (!IsInNeighborhood(point, oldPoint)) return target;
        }
        // すでに他のコマで埋まってたら失敗
        if (scoreArray[point] != 0) return target;
        scoreArray[point] = simpeiColor;
        float y = (point < 16) ? upperWorldHeight : downerWorldHeight;
        target = new Vector3(WP[point,0], y, WP[point,1]);
        return target;
    } 

    public Vector3 PutEnemySimpei(int point, int simpeiColor)
    {
        float y = (point < 16) ? upperWorldHeight : downerWorldHeight;
        scoreArray[point] = simpeiColor;
        Vector3 target = new Vector3(WP[point, 0], y, WP[point,1]);
        return target;
    }

    public int PickUpSimpei(int point, int playerColor)
    {
        if (scoreArray[point] == 0) return -1;
        if (playerColor != scoreArray[point]) return -1;
        scoreArray[point] = 0;
        return point;
    }

    public void PickUpEnemySimpei(int point)
    {
        scoreArray[point] = 0;
    }

    // ============================================
    // 勝利判定とコマを挟んだ時のそれぞれの方向の判定に使う
    // ============================================
    int[,] direction = new int[8,2] {
        {1,0}, {1,1}, {0,1}, {-1,1},
        {-1,0}, {-1,-1}, {0,-1}, {1,-1},
    };
    // 座標番号(0〜25)からxy座標に変換
    int[] getXY(int num, int width){
        return new int[2]{num % width, num / width};
    }
    // xy座標から座標番号に変換
    int getNum(int[] xy, int width){
        return xy[0] + (xy[1] * width);
    }

    // =======================================
    // 勝利判定(青:1/赤:-1/勝者なし:0)
    // とりあえず3個並んだコマの色とマスのリストを返す
    // =======================================
    public Tuple<int, List<int>> CheckThreeChain(int point)
    {
        var winnerTuple = new Tuple<int, List<int>>(0, new List<int>());
        for (int i = 0; i < direction.GetLength(0) / 2; i++)
        {
            winnerTuple = CalcThreeChain(point, new int[] { direction[i, 0], direction[i, 1] });
            if (winnerTuple.Item1 != 0)
            {
                return winnerTuple;
            }
        }
        return winnerTuple;
    }

    Tuple<int, List<int>> CalcThreeChain(int point, int[] dir)
    {
        int winner = 0;
        int width, sum = scoreArray[point];
        int score = sum;
        List<int> winnerPoints = new List<int>();
        // focus point
        int[] fp;
        winnerPoints.Add(point);
        if (point < 16)
        {
            width = 4; fp = getXY(point, width);
        }
        else
        {
            width = 3; fp = getXY(point - 16, width);
        }
        int x, y, n;
        for (int i = 1; i < width; i++)
        {
            x = fp[0] + i * dir[0];
            y = fp[1] + i * dir[1];
            // マスの範囲外ならbreak
            if (x > width - 1 || y > width - 1 || x < 0 || y < 0) break;
            // 座標からマス番号に変換
            if (point < 16) n = getNum(new int[] { x, y }, width);
            else n = 16 + getNum(new int[] { x, y }, width);
            // コマの色が違うときとマスが空のときはbreak
            if (scoreArray[n] + score == 0 || scoreArray[n] == 0) break;
            sum += scoreArray[n];
            winnerPoints.Add(n);
        }
        for (int i = 1; i < width; i++)
        {
            x = fp[0] - i * dir[0];
            y = fp[1] - i * dir[1];
            if (x > width - 1 || y > width - 1 || x < 0 || y < 0) break;
            if (point < 16) n = getNum(new int[] { x, y }, width);
            else n = 16 + getNum(new int[] { x, y }, width);
            if (scoreArray[n] + score == 0 || scoreArray[n] == 0) break;
            sum += scoreArray[n];
            winnerPoints.Add(n);
        }
        if (sum == 3) winner = 1;
        else if (sum == -3) winner = -1;
        return new Tuple<int, List<int>>(winner, winnerPoints);
    }

    // ================================
    // 挟み込み判定
    // ================================

    // 挟み込んだコマのマスのリストを返す(空リスト含む)
    public List<int> CheckSandwitchSimpei(int point)
    {
        List<int> enemiesList = new List<int>();
        List<int> enemies;
        for (int i = 0; i < direction.GetLength(0); i++)
        {
            enemies = CalcSandwitch(point, new int[]{direction[i,0], direction[i, 1]});
            if (enemies.Count != 0)
            {
                enemiesList.AddRange(enemies);
            }
        }   
        return  enemiesList;
    }

    List<int> CalcSandwitch(int point, int[] dir)
    {
        List<int> points = new List<int>();
        int score = scoreArray[point];
        int[] focusXY;
        int width, x, y, n;
        if (point < 16)
        {
            width = 4;
            focusXY = getXY(point, width);
        } 
        else
        {
            width = 3;
            focusXY = getXY(point - 16, width);
        } 
        for (int i = 1; i < width; i++)
        {
            x = focusXY[0] + i * dir[0];
            y = focusXY[1] + i * dir[1];
            if (x > width - 1 || y > width - 1 || x < 0 || y < 0)
            {
                points.Clear();
                break;
            }
            if (point < 16) n = getNum(new int[]{x, y}, width);
            else n = 16 + getNum(new int[]{x, y}, width);
            if (i == 1)
            {
                if (score + scoreArray[n] != 0) break;
                points.Add(n);
            }
            if (i == 2)
            {
                if (scoreArray[n] == 0)
                {
                    points.Clear();
                    break;
                }
                if (i == width-1 && score + scoreArray[n] == 0)
                {
                    points.Clear();
                    break;
                }
                if (score == scoreArray[n]) break;

                points.Add(n);
            }
            if (i == 3)
            {
                if (score != scoreArray[n])
                    points.Clear();
            }
        }
        return points;
    }

    bool IsInStartPoint(int point)
    {
        for (int i = 0; i < startPoints.Length; i++)
        {
            if (point == startPoints[i])
            {
                return true;
            }
        }
        return false;
    }

    bool IsInNeighborhood(int point, int oldPoint)
    {
        if (point == oldPoint) return true;
        for (int i = 0; i < neighborhood[oldPoint].Length; i++)
        {
            if (point == neighborhood[oldPoint][i])
            {
                return true;
            }
        }
        return false;
    }

    // 8ターン目以降に選択したコマが移動可能かどうか
    public bool IsCanMoveToNeighborhood(int point)
    {
        for (int i = 0; i < neighborhood[point].Length; i++)
        {
            int index = neighborhood[point][i];
            if (scoreArray[index] == 0)
            {
                return true;
            }
        }
        return false;
    }

    public Tuple<int[], float[,]> GetStartPoints()
    {
        float[,] startPointInWorld = new float[4, 2];
        for (int i = 0; i < startPoints.Length; i++)
        {
            startPointInWorld[i, 0] = WP[startPoints[i], 0];
            startPointInWorld[i, 1] = WP[startPoints[i], 1];
        }
        var ret = new Tuple<int[], float[,]>(startPoints, startPointInWorld);
        return ret;
    }

    public Tuple<int[], float[,]> GetAvailablePoints(int point)
    {
        int[] neighborhoodPoint = neighborhood[point];
        float[,] availablePointInWorld = new float[neighborhoodPoint.Length, 2];
        for(int i = 0; i < neighborhoodPoint.Length; i++)
        {
            if (scoreArray[neighborhoodPoint[i]] != 0)
            {
                availablePointInWorld[i, 0] = -1;
                availablePointInWorld[i, 1] = -1;
            }
            else
            {
                availablePointInWorld[i, 0] = WP[neighborhoodPoint[i], 0];
                availablePointInWorld[i, 1] = WP[neighborhoodPoint[i], 1];
            }
        }
        var ret = new Tuple<int[], float[,]>(neighborhoodPoint, availablePointInWorld);
        return ret;
    }

    // private void OnGUI()
    // {
    //     string label = "";
    //     string temp = "";
    //     for (int i = 15; i >= 0; i--)
    //     {
    //         temp = $"{scoreArray[i]}, " + temp;
    //         if (i % 4 == 0)
    //         {
    //             label += temp + "\n";
    //             temp = "";
    //         }
    //     }
    //     for (int i = 24; i >= 16; i--)
    //     {
    //         temp = $"{scoreArray[i]}, " + temp;
    //         if ((i-16) % 3 == 0)
    //         {
    //             label += temp + "\n";
    //             temp = "";
    //         }
    //     }

    //     GUI.color = Color.white;
    //     GUI.Label(new Rect(0, 60, 300, 600), label);
    // }
}
