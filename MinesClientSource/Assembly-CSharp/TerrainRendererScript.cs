using System;
using UnityEngine;

public class TerrainRendererScript : MonoBehaviour
{
    private void Start()
    {
        CellRender.InitCaches();
        this.mf = base.gameObject.GetComponent<MeshFilter>();
        this.mesh = new Mesh();
        this.mesh.MarkDynamic();
        this.mf.mesh = this.mesh;
        this.InitMeshArrays();
        this.CreateMesh();
        this.LinkMesh();
        this._meshInited = true;
    }

    public void SetMaps(MapModel _map)
    {
        map = _map;
        inited = true;
    }

    private void LinkMesh()
    {
        if (this._meshInited)
        {
            this.mesh.triangles = this._fakeTriangles;
        }
        this.mesh.vertices = this._vertices;
        this.mesh.triangles = this._triangles;
        this.mesh.colors = this._colors;
        this.mesh.uv = this._shadowUVs;
        this.mesh.uv2 = this._infoUVs;
        this.mesh.uv3 = this._coordUVs;
        this.mesh.uv4 = this._coordUV2s;
        this.mesh.uv5 = this._anim1UVs;
        this.mesh.uv6 = this._anim2UVs;
    }

    private void InitMeshArrays()
    {
        this._fakeTriangles = new int[0];
        this._triangles = new int[this.xSize * this.ySize * 6];
        this._dists = new Vector4[(this.xSize + 1) * (this.ySize + 1)];
        this._vertices = new Vector3[4 * this.xSize * this.ySize];
        this._swap_vertices = new Vector3[4 * this.xSize * this.ySize];
        this._shadowUVs = new Vector2[4 * this.xSize * this.ySize];
        this._coordUVs = new Vector2[4 * this.xSize * this.ySize];
        this._coordUV2s = new Vector2[4 * this.xSize * this.ySize];
        this._infoUVs = new Vector2[4 * this.xSize * this.ySize];
        this._anim1UVs = new Vector2[4 * this.xSize * this.ySize];
        this._anim2UVs = new Vector2[4 * this.xSize * this.ySize];
        this._infoUVs = new Vector2[4 * this.xSize * this.ySize];
        this._colors = new Color[4 * this.xSize * this.ySize];
    }

    private float randxd(int x, int y)
    {
        int num = (5 * x + 11 * y) * (13 * x + 7 * y) % 3221;
        return (float)(num * num % 7);
    }

    private float randyd(int x, int y)
    {
        int num = (17 * x + 19 * y) * (23 * x + 37 * y) % 3469;
        return (float)(num * num % 7);
    }

    private int sandType(int x, int y)
    {
        int cell = map.GetCell(x, y - 1);
        int cell2 = map.GetCell(x - 1, y);
        int cell3 = map.GetCell(x + 1, y);
        int cell4 = map.GetCell(x, y + 1);
        return (CellRender.isSand(cell) ? 1 : 0) + (CellRender.isSand(cell2) ? 2 : 0) + (CellRender.isSandOrBase(cell4) ? 4 : 0) + (CellRender.isSand(cell3) ? 8 : 0);
    }

    private Vector4 GetDistortion(int x, int y)
    {
        if (ClientConfig.noDistortion)
        {
            return default;
        }
        int cell = map.GetCell(x - 1, y - 1);
        int cell2 = map.GetCell(x, y - 1);
        int cell3 = map.GetCell(x - 1, y);
        int cell4 = map.GetCell(x, y);
        int cell5 = map.GetCell(x + 1, y);
        int cell6 = map.GetCell(x, y + 1);
        float w = 0f;
        int num = CellRender.reliefType(cell4);
        if (num > 0)
        {
            w = (float)(((num == CellRender.reliefType(cell2)) ? 1 : 0) + ((num == CellRender.reliefType(cell3)) ? 2 : 0) + ((num == CellRender.reliefType(cell6)) ? 4 : 0) + ((num == CellRender.reliefType(cell5)) ? 8 : 0));
        }
        float z = 0f;
        if ((CellRender.haveShadow(cell) || CellRender.haveShadow(cell2) || CellRender.haveShadow(cell3) || CellRender.haveShadow(cell4)) && (CellRender.receiveShadow(cell) || CellRender.receiveShadow(cell2) || CellRender.receiveShadow(cell3) || CellRender.receiveShadow(cell4)))
        {
            z = 0.7f;
        }
        if (CellRender.isDistortion(cell) && CellRender.isDistortion(cell4) && CellRender.isDistortion(cell2) && CellRender.isDistortion(cell3))
        {
            return new Vector4((this.randxd(x, y) - 3f) / 16f, (this.randyd(x, y) - 3f) / 16f, z, w);
        }
        if (y == 0 || CellRender.isNoDistortion(cell) || CellRender.isNoDistortion(cell2) || CellRender.isNoDistortion(cell3) || CellRender.isNoDistortion(cell4) || (CellRender.isDistortion(cell) && CellRender.isDistortion(cell4)) || (CellRender.isDistortion(cell2) && CellRender.isDistortion(cell3)))
        {
            return new Vector4(0f, 0f, z, w);
        }
        if (CellRender.isDistortion(cell) && CellRender.isDistortion(cell2))
        {
            return new Vector4(0f, this.randyd(x, y) / 16f, z, w);
        }
        if (CellRender.isDistortion(cell) && CellRender.isDistortion(cell3))
        {
            return new Vector4(-this.randxd(x, y) / 16f, 0f, z, w);
        }
        if (CellRender.isDistortion(cell2) && CellRender.isDistortion(cell4))
        {
            return new Vector4(this.randxd(x, y) / 16f, 0f, z, w);
        }
        if (CellRender.isDistortion(cell3) && CellRender.isDistortion(cell4))
        {
            return new Vector4(0f, -this.randyd(x, y) / 16f, z, w);
        }
        if (CellRender.isDistortion(cell))
        {
            return new Vector4(-this.randxd(x, y) / 16f, this.randyd(x, y) / 16f, z, w);
        }
        if (CellRender.isDistortion(cell2))
        {
            return new Vector4(this.randxd(x, y) / 16f, this.randyd(x, y) / 16f, z, w);
        }
        if (CellRender.isDistortion(cell3))
        {
            return new Vector4(-this.randxd(x, y) / 16f, -this.randyd(x, y) / 16f, z, w);
        }
        if (CellRender.isDistortion(cell4))
        {
            return new Vector4(this.randxd(x, y) / 16f, -this.randyd(x, y) / 16f, z, w);
        }
        return new Vector4(0f, 0f, z, w);
    }

    private bool IsBuilding(int cell)
    {
        return cell == 106;
    }

    private bool IsBuildingCorner(int cell)
    {
        return cell == 38;
    }

    private bool IsBuildingDoor(int cell)
    {
        return cell == 37;
    }

    private bool IsBuildingOrDoor(int cell)
    {
        return this.IsBuildingDoor(cell) || this.IsBuilding(cell);
    }

    public void RecreateMeshes()
    {
        Vector3 position = new Vector3(0f, 0f, 0f);
        Vector3 position2 = new Vector3(0f, 100f, 0f);
        Vector3 a = Camera.main.WorldToScreenPoint(position);
        Vector3 b = Camera.main.WorldToScreenPoint(position2);
        float oldOrthographicSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = Camera.main.orthographicSize * (a - b).magnitude / (unitSize * 100f);
        this.xSize = Mathf.FloorToInt((float)this.lastScreenWidth / unitSize) + 2 * this.PADDING + 2;
        this.ySize = Mathf.FloorToInt((float)this.lastScreenHeight / unitSize) + 2 * this.PADDING + 2;
        needUpdate = true;
        this.InitMeshArrays();
        this.CreateMesh();
        this.LinkMesh();
    }

    private void UpdateCamera()
    {
        if (Screen.width != this.lastScreenWidth || Screen.height != this.lastScreenHeight)
        {
            this.lastScreenHeight = Screen.height;
            this.lastScreenWidth = Screen.width;
            this.RecreateMeshes();
        }
        Vector3 position = Camera.main.transform.position;
        double num = (double)unitSize;
        double num2 = (double)this.cx;
        double num3 = (double)this.cy;
        num2 = (Math.Floor(num2 * num) + (double)(Screen.width % 2) * 0.5) / num;
        num3 = (Math.Floor(num3 * num) + (double)(Screen.height % 2) * 0.5) / num;
        this.cx = (float)num2;
        this.cy = (float)num3;
        position.x = this.cx;
        position.y = -this.cy;
        Camera.main.transform.position = position;
    }

    private void UpdateMesh()
    {
        this.lastCx = this.cx;
        this.lastCy = this.cy;
        int num = Mathf.FloorToInt(this.cx - (float)(this.xSize / 2));
        int num2 = Mathf.FloorToInt(this.cy - (float)(this.ySize / 2));
        for (int i = 0; i < this.xSize + 1; i++)
        {
            for (int j = 0; j < this.ySize + 1; j++)
            {
                this._dists[j * (this.xSize + 1) + i] = this.GetDistortion(num + i, num2 + j);
            }
        }
        for (int k = 0; k < this.xSize; k++)
        {
            for (int l = 0; l < this.ySize; l++)
            {
                int vert = 4 * (l * this.xSize + k);
                int cell = map.GetCell(num + k, num2 + l);
                this._vertices[vert].x = (float)num + this.quadSize * (float)k + this._dists[l * (this.xSize + 1) + k].x;
                this._vertices[vert].y = -(float)num2 - this.quadSize * (float)l + this._dists[l * (this.xSize + 1) + k].y;
                this._vertices[vert + 1].x = (float)num + this.quadSize * (float)(k + 1) + this._dists[l * (this.xSize + 1) + (k + 1)].x;
                this._vertices[vert + 1].y = -(float)num2 - this.quadSize * (float)l + this._dists[l * (this.xSize + 1) + (k + 1)].y;
                this._vertices[vert + 2].x = (float)num + this.quadSize * (float)k + this._dists[(l + 1) * (this.xSize + 1) + k].x;
                this._vertices[vert + 2].y = -(float)num2 - this.quadSize * (float)(l + 1) + this._dists[(l + 1) * (this.xSize + 1) + k].y;
                this._vertices[vert + 3].x = (float)num + this.quadSize * (float)(k + 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].x;
                this._vertices[vert + 3].y = -(float)num2 - this.quadSize * (float)(l + 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].y;
                float num3 = CellRender.cellConfigs[cell].textureType;
                float animType = CellRender.cellConfigs[cell].animType;
                float animSpeed = CellRender.cellConfigs[cell].animSpeed;
                int wx = CellRender.cellConfigs[cell].wx;
                int wy = CellRender.cellConfigs[cell].wy;
                int dx = CellRender.cellConfigs[cell].dx;
                int dy = CellRender.cellConfigs[cell].dy;
                int wx2 = CellRender.cellConfigs[cell].wx2;
                int wy2 = CellRender.cellConfigs[cell].wy2;
                int dx2 = CellRender.cellConfigs[cell].dx2;
                int dy2 = CellRender.cellConfigs[cell].dy2;
                int cell2 = map.GetCell(num + k, num2 + l - 1);
                int cell3 = map.GetCell(num + k - 1, num2 + l);
                int cell4 = map.GetCell(num + k + 1, num2 + l);
                int cell5 = map.GetCell(num + k, num2 + l + 1);
                int cell6 = map.GetCell(num + k - 1, num2 + l - 1);
                int cell7 = map.GetCell(num + k - 1, num2 + l + 1);
                int cell8 = map.GetCell(num + k + 1, num2 + l - 1);
                int cell9 = map.GetCell(num + k + 1, num2 + l + 1);
                bool flag = false;
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                bool flag2 = false;
                if (this.IsBuilding(cell))
                {
                    flag = true;
                    switch ((this.IsBuildingOrDoor(cell2) ? 1 : 0) + (this.IsBuildingOrDoor(cell3) ? 2 : 0) + (this.IsBuildingOrDoor(cell5) ? 4 : 0) + (this.IsBuildingOrDoor(cell4) ? 8 : 0))
                    {
                        case 0:
                            num4 = 5;
                            num5 = 0;
                            break;
                        case 1:
                            num4 = 4;
                            num5 = 1;
                            num6 = 1;
                            break;
                        case 2:
                            num4 = 4;
                            num5 = 1;
                            num6 = 0;
                            break;
                        case 3:
                            if (this.IsBuildingOrDoor(cell6))
                            {
                                if (this.IsBuildingOrDoor(cell7))
                                {
                                    num4 = 0;
                                    num5 = 1;
                                    num6 = 2;
                                }
                                else if (this.IsBuildingOrDoor(cell8))
                                {
                                    flag2 = true;
                                    num4 = 0;
                                    num5 = 1;
                                    num6 = 1;
                                }
                                else
                                {
                                    num4 = 0;
                                    num5 = 2;
                                    num6 = 3;
                                }
                            }
                            else
                            {
                                num4 = 1;
                                num5 = 1;
                            }
                            break;
                        case 4:
                            num4 = 4;
                            num5 = 1;
                            num6 = 3;
                            break;
                        case 5:
                            num4 = 5;
                            num5 = 1;
                            break;
                        case 6:
                            if (this.IsBuildingOrDoor(cell7))
                            {
                                if (this.IsBuildingOrDoor(cell9))
                                {
                                    num4 = 0;
                                    num5 = 1;
                                    num6 = 1;
                                }
                                else if (this.IsBuildingOrDoor(cell6))
                                {
                                    num4 = 0;
                                    num5 = 1;
                                    flag2 = true;
                                    num6 = 0;
                                }
                                else
                                {
                                    num4 = 0;
                                    num5 = 2;
                                    num6 = 2;
                                }
                            }
                            else
                            {
                                num4 = 1;
                                num5 = 1;
                            }
                            break;
                        case 7:
                            num4 = 1;
                            num5 = 2;
                            num6 = 3;
                            break;
                        case 8:
                            num4 = 4;
                            num5 = 1;
                            num6 = 2;
                            break;
                        case 9:
                            if (this.IsBuildingOrDoor(cell8))
                            {
                                if (this.IsBuildingOrDoor(cell6))
                                {
                                    num4 = 0;
                                    num5 = 1;
                                    num6 = 3;
                                }
                                else if (this.IsBuildingOrDoor(cell9))
                                {
                                    num4 = 0;
                                    num5 = 1;
                                    flag2 = true;
                                    num6 = 2;
                                }
                                else
                                {
                                    num4 = 0;
                                    num5 = 2;
                                    num6 = 0;
                                }
                            }
                            else
                            {
                                num4 = 1;
                                num5 = 1;
                            }
                            break;
                        case 10:
                            num4 = 5;
                            num5 = 1;
                            break;
                        case 11:
                            num4 = 1;
                            num5 = 2;
                            num6 = 0;
                            break;
                        case 12:
                            if (this.IsBuildingOrDoor(cell9))
                            {
                                if (this.IsBuildingOrDoor(cell7))
                                {
                                    num4 = 0;
                                    num5 = 1;
                                    flag2 = true;
                                    num6 = 3;
                                }
                                else if (this.IsBuildingOrDoor(cell8))
                                {
                                    num4 = 0;
                                    num5 = 1;
                                }
                                else
                                {
                                    num4 = 0;
                                    num5 = 2;
                                    num6 = 1;
                                }
                            }
                            else
                            {
                                num4 = 1;
                                num5 = 1;
                            }
                            break;
                        case 13:
                            num4 = 1;
                            num5 = 2;
                            num6 = 1;
                            break;
                        case 14:
                            num4 = 1;
                            num5 = 2;
                            num6 = 2;
                            break;
                        case 15:
                            num4 = 1;
                            num5 = 1;
                            break;
                    }
                }
                else if (this.IsBuildingDoor(cell))
                {
                    flag = true;
                    int num7 = (this.IsBuildingOrDoor(cell2) ? 1 : 0) + (this.IsBuildingOrDoor(cell3) ? 2 : 0) + (this.IsBuildingOrDoor(cell5) ? 4 : 0) + (this.IsBuildingOrDoor(cell4) ? 8 : 0);
                    if (num7 != 7)
                    {
                        switch (num7)
                        {
                            case 11:
                                num4 = 2;
                                num5 = 2;
                                num6 = 0;
                                goto IL_89A;
                            case 13:
                                num4 = 2;
                                num5 = 2;
                                num6 = 1;
                                goto IL_89A;
                            case 14:
                                num4 = 2;
                                num5 = 2;
                                num6 = 2;
                                goto IL_89A;
                        }
                        num4 = 1;
                        num5 = 1;
                    }
                    else
                    {
                        num4 = 2;
                        num5 = 2;
                        num6 = 3;
                    }
                }
                else if (this.IsBuildingCorner(cell))
                {
                    flag = true;
                    int num8 = (this.IsBuildingOrDoor(cell2) ? 1 : 0) + (this.IsBuildingOrDoor(cell3) ? 2 : 0) + (this.IsBuildingOrDoor(cell5) ? 4 : 0) + (this.IsBuildingOrDoor(cell4) ? 8 : 0);
                    if (num8 <= 6)
                    {
                        if (num8 == 3)
                        {
                            num4 = 0;
                            num5 = 0;
                            num6 = 2;
                            goto IL_89A;
                        }
                        if (num8 == 6)
                        {
                            num4 = 0;
                            num5 = 0;
                            num6 = 1;
                            goto IL_89A;
                        }
                    }
                    else
                    {
                        if (num8 == 9)
                        {
                            num4 = 0;
                            num5 = 0;
                            num6 = 3;
                            goto IL_89A;
                        }
                        if (num8 == 12)
                        {
                            num4 = 0;
                            num5 = 0;
                            num6 = 0;
                            goto IL_89A;
                        }
                    }
                    num4 = 5;
                    num5 = 1;
                }
            IL_89A:
                if (flag)
                {
                    this._vertices[vert].z = -5f;
                    this._vertices[vert + 1].z = -5f;
                    this._vertices[vert + 2].z = -5f;
                    this._vertices[vert + 3].z = -5f;
                }
                else
                {
                    this._vertices[vert].z = 0f;
                    this._vertices[vert + 1].z = 0f;
                    this._vertices[vert + 2].z = 0f;
                    this._vertices[vert + 3].z = 0f;
                }
                if (flag)
                {
                    Vector2 vector = new Vector2((float)(33 + num4), (float)(69 + num5));
                    Vector2 vector2 = new Vector2((float)(33 + num4 + 1), (float)(69 + num5));
                    Vector2 vector3 = new Vector2((float)(33 + num4), (float)(69 + num5 + 1));
                    Vector2 vector4 = new Vector2((float)(33 + num4 + 1), (float)(69 + num5 + 1));
                    if (flag2)
                    {
                        num6 += 4;
                    }

                    var _coord = this.MODE == 2 ? _coordUVs : _coordUV2s;

                    switch (num6)
                    {
                        case 0:
                            _coord[vert] = vector;
                            _coord[vert + 1] = vector2;
                            _coord[vert + 2] = vector3;
                            _coord[vert + 3] = vector4;
                            break;
                        case 1:
                            _coord[vert] = vector3;
                            _coord[vert + 1] = vector;
                            _coord[vert + 2] = vector4;
                            _coord[vert + 3] = vector2;
                            break;
                        case 2:
                            _coord[vert] = vector4;
                            _coord[vert + 1] = vector3;
                            _coord[vert + 2] = vector2;
                            _coord[vert + 3] = vector;
                            break;
                        case 3:
                            _coord[vert] = vector2;
                            _coord[vert + 1] = vector4;
                            _coord[vert + 2] = vector;
                            _coord[vert + 3] = vector3;
                            break;
                        case 4:
                            _coord[vert] = vector2;
                            _coord[vert + 1] = vector;
                            _coord[vert + 2] = vector4;
                            _coord[vert + 3] = vector3;
                            break;
                        case 5:
                            _coord[vert] = vector4;
                            _coord[vert + 1] = vector2;
                            _coord[vert + 2] = vector3;
                            _coord[vert + 3] = vector;
                            break;
                        case 6:
                            _coord[vert] = vector3;
                            _coord[vert + 1] = vector4;
                            _coord[vert + 2] = vector;
                            _coord[vert + 3] = vector2;
                            break;
                        case 7:
                            _coord[vert] = vector;
                            _coord[vert + 1] = vector3;
                            _coord[vert + 2] = vector2;
                            _coord[vert + 3] = vector4;
                            break;
                    }
                    num3 = 5f;
                }

                if (this.MODE == 2)
                {

                    Color color = default(Color);
                    if (animType == 3f || animType == 4f || animType == 6f || animType == 8f)
                    {
                        color = new Color(1f, 0f, 0f);
                    }
                    else if (animType == 7f)
                    {
                        color = new Color(0f, 1f, 0f);
                    }
                    int num9 = Mathf.FloorToInt((float)(100 + num) + this.quadSize * (float)k) % wx + dx;
                    int num10 = Mathf.FloorToInt((float)(128000 - num2) - this.quadSize * (float)l) % wy + dy + 1;
                    if (!flag)
                    {
                        this._coordUVs[vert].x = (float)num9 + this._dists[l * (this.xSize + 1) + k].x;
                        this._coordUVs[vert].y = (float)num10 + this._dists[l * (this.xSize + 1) + k].y;
                        this._coordUVs[vert + 1].x = (float)(num9 + 1) + this._dists[l * (this.xSize + 1) + (k + 1)].x;
                        this._coordUVs[vert + 1].y = (float)num10 + this._dists[l * (this.xSize + 1) + (k + 1)].y;
                        this._coordUVs[vert + 2].x = (float)num9 + this._dists[(l + 1) * (this.xSize + 1) + k].x;
                        this._coordUVs[vert + 2].y = (float)(num10 - 1) + this._dists[(l + 1) * (this.xSize + 1) + k].y;
                        this._coordUVs[vert + 3].x = (float)(num9 + 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].x;
                        this._coordUVs[vert + 3].y = (float)(num10 - 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].y;
                    }
                    this._colors[vert] = color;
                    this._colors[vert + 1] = color;
                    this._colors[vert + 2] = color;
                    this._colors[vert + 3] = color;
                    if (num3 == 16f || num3 == 32f)
                    {
                        num3 += (float)this.sandType(num + k, num2 + l);
                        int num11 = Mathf.FloorToInt((float)(100 + num) + this.quadSize * (float)k) % wx2 + dx2;
                        int num12 = Mathf.FloorToInt((float)(128000 - num2) - this.quadSize * (float)l) % wy2 + dy2 + 1;
                        this._coordUVs[vert].x = (float)num11 + this._dists[l * (this.xSize + 1) + k].x;
                        this._coordUVs[vert].y = (float)num12 + this._dists[l * (this.xSize + 1) + k].y;
                        this._coordUVs[vert + 1].x = (float)(num11 + 1) + this._dists[l * (this.xSize + 1) + (k + 1)].x;
                        this._coordUVs[vert + 1].y = (float)num12 + this._dists[l * (this.xSize + 1) + (k + 1)].y;
                        this._coordUVs[vert + 2].x = (float)num11 + this._dists[(l + 1) * (this.xSize + 1) + k].x;
                        this._coordUVs[vert + 2].y = (float)(num12 - 1) + this._dists[(l + 1) * (this.xSize + 1) + k].y;
                        this._coordUVs[vert + 3].x = (float)(num11 + 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].x;
                        this._coordUVs[vert + 3].y = (float)(num12 - 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].y;
                    }
                }
                else
                {
                    Color col = CellRender.cellConfigs[cell].col;
                    int num9 = Mathf.FloorToInt((float)(100 + num) + this.quadSize * (float)k) % wx + dx;
                    int num10 = Mathf.FloorToInt((float)(128000 - num2) - this.quadSize * (float)l) % wy + dy + 1;
                    this._coordUVs[vert].x = (float)num9 + this._dists[l * (this.xSize + 1) + k].x;
                    this._coordUVs[vert].y = (float)num10 + this._dists[l * (this.xSize + 1) + k].y;
                    this._coordUVs[vert + 1].x = (float)(num9 + 1) + this._dists[l * (this.xSize + 1) + (k + 1)].x;
                    this._coordUVs[vert + 1].y = (float)num10 + this._dists[l * (this.xSize + 1) + (k + 1)].y;
                    this._coordUVs[vert + 2].x = (float)num9 + this._dists[(l + 1) * (this.xSize + 1) + k].x;
                    this._coordUVs[vert + 2].y = (float)(num10 - 1) + this._dists[(l + 1) * (this.xSize + 1) + k].y;
                    this._coordUVs[vert + 3].x = (float)(num9 + 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].x;
                    this._coordUVs[vert + 3].y = (float)(num10 - 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].y;
                    if (animType == 2f || animType == 5f)
                    {
                        int num11 = Mathf.FloorToInt((float)(100 + num) + this.quadSize * (float)k) % wx2 + dx2;
                        int num12 = Mathf.FloorToInt((float)(128000 - num2) - this.quadSize * (float)l) % wy2 + dy2 + 1;
                        this._anim1UVs[vert].x = (float)num11 + this._dists[l * (this.xSize + 1) + k].x;
                        this._anim1UVs[vert].y = (float)num12 + this._dists[l * (this.xSize + 1) + k].y;
                        this._anim1UVs[vert + 1].x = (float)(num11 + 1) + this._dists[l * (this.xSize + 1) + (k + 1)].x;
                        this._anim1UVs[vert + 1].y = (float)num12 + this._dists[l * (this.xSize + 1) + (k + 1)].y;
                        this._anim1UVs[vert + 2].x = (float)num11 + this._dists[(l + 1) * (this.xSize + 1) + k].x;
                        this._anim1UVs[vert + 2].y = (float)(num12 - 1) + this._dists[(l + 1) * (this.xSize + 1) + k].y;
                        this._anim1UVs[vert + 3].x = (float)(num11 + 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].x;
                        this._anim1UVs[vert + 3].y = (float)(num12 - 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].y;
                    }
                    else if (animType == 6f || animType == 8f)
                    {
                        this._anim1UVs[vert].x = (float)dx2;
                        this._anim1UVs[vert + 2].x = (float)dx2;
                        this._anim1UVs[vert + 3].x = (float)dx2;
                        this._anim1UVs[vert + 1].x = (float)dx2;
                        this._anim1UVs[vert].y = (float)dy2;
                        this._anim1UVs[vert + 1].y = (float)dy2;
                        this._anim1UVs[vert + 2].y = (float)dy2;
                        this._anim1UVs[vert + 3].y = (float)dy2;
                    }
                    this._colors[vert] = col;
                    this._colors[vert + 1] = col;
                    this._colors[vert + 2] = col;
                    this._colors[vert + 3] = col;

                    this._anim2UVs[vert] = new Vector2(animType, animSpeed);
                    this._anim2UVs[vert + 1] = new Vector2(animType, animSpeed);
                    this._anim2UVs[vert + 2] = new Vector2(animType, animSpeed);
                    this._anim2UVs[vert + 3] = new Vector2(animType, animSpeed);
                    if (num3 == 0f)
                    {
                        this._infoUVs[vert] = new Vector3(num3, this._dists[l * (this.xSize + 1) + k].z);
                        this._infoUVs[vert + 1] = new Vector3(num3, this._dists[l * (this.xSize + 1) + (k + 1)].z);
                        this._infoUVs[vert + 2] = new Vector3(num3, this._dists[(l + 1) * (this.xSize + 1) + k].z);
                        this._infoUVs[vert + 3] = new Vector3(num3, this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].z);
                    }
                    else if (num3 == 1f)
                    {
                        float w = this._dists[l * (this.xSize + 1) + k].w;
                        this._infoUVs[vert] = new Vector3(num3, w);
                        this._infoUVs[vert + 1] = new Vector3(num3, w);
                        this._infoUVs[vert + 2] = new Vector3(num3, w);
                        this._infoUVs[vert + 3] = new Vector3(num3, w);
                    }
                    else if (num3 == 16f || num3 == 32f)
                    {
                        num3 += (float)this.sandType(num + k, num2 + l);
                        this._infoUVs[vert] = new Vector3(num3, this._dists[l * (this.xSize + 1) + k].z);
                        this._infoUVs[vert + 1] = new Vector3(num3, this._dists[l * (this.xSize + 1) + (k + 1)].z);
                        this._infoUVs[vert + 2] = new Vector3(num3, this._dists[(l + 1) * (this.xSize + 1) + k].z);
                        this._infoUVs[vert + 3] = new Vector3(num3, this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].z);
                        int num13 = Mathf.FloorToInt((float)(100 + num) + this.quadSize * (float)k) % wx2 + dx2;
                        int num14 = Mathf.FloorToInt((float)(128000 - num2) - this.quadSize * (float)l) % wy2 + dy2 + 1;
                        this._coordUV2s[vert].x = (float)num13 + this._dists[l * (this.xSize + 1) + k].x;
                        this._coordUV2s[vert].y = (float)num14 + this._dists[l * (this.xSize + 1) + k].y;
                        this._coordUV2s[vert + 1].x = (float)(num13 + 1) + this._dists[l * (this.xSize + 1) + (k + 1)].x;
                        this._coordUV2s[vert + 1].y = (float)num14 + this._dists[l * (this.xSize + 1) + (k + 1)].y;
                        this._coordUV2s[vert + 2].x = (float)num13 + this._dists[(l + 1) * (this.xSize + 1) + k].x;
                        this._coordUV2s[vert + 2].y = (float)(num14 - 1) + this._dists[(l + 1) * (this.xSize + 1) + k].y;
                        this._coordUV2s[vert + 3].x = (float)(num13 + 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].x;
                        this._coordUV2s[vert + 3].y = (float)(num14 - 1) + this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].y;
                    }
                    else
                    {
                        this._infoUVs[vert] = new Vector3(num3, this._dists[l * (this.xSize + 1) + k].z);
                        this._infoUVs[vert + 1] = new Vector3(num3, this._dists[l * (this.xSize + 1) + (k + 1)].z);
                        this._infoUVs[vert + 2] = new Vector3(num3, this._dists[(l + 1) * (this.xSize + 1) + k].z);
                        this._infoUVs[vert + 3] = new Vector3(num3, this._dists[(l + 1) * (this.xSize + 1) + (k + 1)].z);
                    }
                }
            }
        }
    }

    private void CreateMesh()
    {
        for (int i = 0; i < this.xSize; i++)
        {
            for (int j = 0; j < this.ySize; j++)
            {
                this._vertices[4 * (j * this.xSize + i)] = new Vector3(this.quadSize * (float)i, this.quadSize * (float)j);
                this._vertices[4 * (j * this.xSize + i) + 1] = new Vector3(this.quadSize * (float)(i + 1), this.quadSize * (float)j);
                this._vertices[4 * (j * this.xSize + i) + 2] = new Vector3(this.quadSize * (float)i, this.quadSize * (float)(j + 1));
                this._vertices[4 * (j * this.xSize + i) + 3] = new Vector3(this.quadSize * (float)(i + 1), this.quadSize * (float)(j + 1));
                float num = Mathf.Sqrt(2f);
                this._shadowUVs[4 * (j * this.xSize + i)] = new Vector2(1f / num, 1f / num);
                this._shadowUVs[4 * (j * this.xSize + i) + 1] = new Vector2(-1f / num, 1f / num);
                this._shadowUVs[4 * (j * this.xSize + i) + 2] = new Vector2(1f / num, -1f / num);
                this._shadowUVs[4 * (j * this.xSize + i) + 3] = new Vector2(-1f / num, -1f / num);
                this._infoUVs[4 * (j * this.xSize + i)] = new Vector2(0f, 0f);
                this._infoUVs[4 * (j * this.xSize + i) + 1] = new Vector2(0f, 0f);
                this._infoUVs[4 * (j * this.xSize + i) + 2] = new Vector2(0f, 0f);
                this._infoUVs[4 * (j * this.xSize + i) + 3] = new Vector2(0f, 0f);
                this._anim1UVs[4 * (j * this.xSize + i)] = new Vector2(0f, 0f);
                this._anim1UVs[4 * (j * this.xSize + i) + 1] = new Vector2(0f, 0f);
                this._anim1UVs[4 * (j * this.xSize + i) + 2] = new Vector2(0f, 0f);
                this._anim1UVs[4 * (j * this.xSize + i) + 3] = new Vector2(0f, 0f);
                this._anim2UVs[4 * (j * this.xSize + i)] = new Vector2(0f, 0f);
                this._anim2UVs[4 * (j * this.xSize + i) + 1] = new Vector2(0f, 0f);
                this._anim2UVs[4 * (j * this.xSize + i) + 2] = new Vector2(0f, 0f);
                this._anim2UVs[4 * (j * this.xSize + i) + 3] = new Vector2(0f, 0f);
                this._coordUVs[4 * (j * this.xSize + i)] = new Vector3(0f, 0f);
                this._coordUVs[4 * (j * this.xSize + i) + 1] = new Vector3(0f, 0f);
                this._coordUVs[4 * (j * this.xSize + i) + 2] = new Vector3(0f, 0f);
                this._coordUVs[4 * (j * this.xSize + i) + 3] = new Vector3(0f, 0f);
                Color color = new Color(0f, 0f, 0f);
                this._colors[4 * (j * this.xSize + i)] = color;
                this._colors[4 * (j * this.xSize + i) + 1] = color;
                this._colors[4 * (j * this.xSize + i) + 2] = color;
                this._colors[4 * (j * this.xSize + i) + 3] = color;
                this._triangles[6 * (j * this.xSize + i)] = 4 * (j * this.xSize + i);
                this._triangles[6 * (j * this.xSize + i) + 1] = 4 * (j * this.xSize + i) + 3;
                this._triangles[6 * (j * this.xSize + i) + 2] = 4 * (j * this.xSize + i) + 2;
                this._triangles[6 * (j * this.xSize + i) + 3] = 4 * (j * this.xSize + i);
                this._triangles[6 * (j * this.xSize + i) + 4] = 4 * (j * this.xSize + i) + 1;
                this._triangles[6 * (j * this.xSize + i) + 5] = 4 * (j * this.xSize + i) + 3;
            }
        }
    }

    public void ChangeQualityFor(int quality)
    {
        this.MODE = quality;
        base.GetComponent<MeshRenderer>().material = this.QualityMaterials[quality];
        this.RecreateMeshes();
    }

    private void Update()
    {
        if (map == null)
        {
            return;
        }
        this.UpdateCamera();
        if (alwaysUpdate)
        {
            needUpdate = true;
        }
        if (needUpdate || Mathf.Abs(this.lastCx - this.cx) + Mathf.Abs(this.lastCy - this.cy) > (float)this.PADDING)
        {
            this.boundsCenter.x = this.cx;
            this.boundsCenter.y = -this.cy;
            this.bounds.center = this.boundsCenter;
            this.boundsSize.x = (float)this.xSize;
            this.boundsSize.y = (float)this.ySize;
            this.boundsSize.z = 10f;
            this.bounds.size = this.boundsSize;
            this.mesh.bounds = this.bounds;
            switch (this.MODE)
            {
                case 0:
                    this.UpdateMesh();
                    this.mesh.vertices = this._vertices;
                    this.mesh.colors = this._colors;
                    this.mesh.uv2 = this._infoUVs;
                    this.mesh.uv3 = this._coordUVs;
                    this.mesh.uv4 = this._coordUV2s;
                    this.mesh.uv5 = this._anim1UVs;
                    this.mesh.uv6 = this._anim2UVs;
                    break;
                case 1:
                    this.UpdateMesh();
                    this.mesh.vertices = this._vertices;
                    this.mesh.colors = this._colors;
                    this.mesh.uv2 = this._infoUVs;
                    this.mesh.uv3 = this._coordUVs;
                    this.mesh.uv4 = this._coordUV2s;
                    break;
                case 2:
                    this.UpdateMesh();
                    this.mesh.vertices = this._vertices;
                    this.mesh.colors = this._colors;
                    this.mesh.uv = this._coordUVs;
                    break;
            }
            needUpdate = false;
        }
    }

    public static bool inited = false;

    public static MapModel map;

    public static bool needUpdate = false;

    public static bool alwaysUpdate = false;

    public static float unitSize = 16f;

    private bool _meshInited;

    private MeshFilter mf;

    private float quadSize = 1f;

    private int xSize = 70;

    private int ySize = 40;

    private Vector4[] _dists;

    private Vector2[] _shadowUVs;

    private Vector2[] _anim1UVs;

    private Vector2[] _anim2UVs;

    private Vector2[] _coordUVs;

    private Vector2[] _coordUV2s;

    private Vector2[] _infoUVs;

    private Vector3[] _vertices;

    private Vector3[] _swap_vertices;

    private Color[] _colors;

    private int[] _triangles;

    private int[] _fakeTriangles;

    private Mesh mesh;

    private float lastCx = -1000f;

    private float lastCy = 1000f;

    private int PADDING = 2;

    public static Vector3 pfd = default(Vector3);

    public float cx = 1f;

    public float cy = 1f;

    public float cr;

    private int lastScreenWidth;

    private int lastScreenHeight;

    private Bounds bounds;

    private Vector3 boundsCenter;

    private Vector3 boundsSize;

    private float lastForceUpdate;

    private const int MODE_NORMAL = 0;

    private const int MODE_POTATO = 1;

    private const int MODE_BEETROOT = 2;

    private int MODE;

    public Material[] QualityMaterials;
}
