using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace B3.BoardSystem
{
    /* Cand vine vorba de pozitia unui vertex sau Edge, trebuie sa iei in considerare faptul ca exista mai multe reprezentari a pozitiei.
     * Un vertex apartine la 3 hex-uri diferite (maxim) si pozitia este in functie de un hex (see HexVertexDir)
     * Deci un vertex are (maxim) 3 (reprezentari de) pozitii diferite
     * Un caz asemanator intalnim si la Edge dar un Edge apartine la (maxim) 2 hex-uri diferite
     * Deci are (maxi) 2 (reprezentari de) pozitii diferite care depind hex (See HexEdgeDir)
     *
     * 
     */
    public class FullHexGrid<Cell, Vertex, Edge> 
        where Cell   : class 
        where Vertex : class
        where Edge   : class
    {
        private class InnerCell
        {
            internal Cell Cell { set; get; }
            internal Vertex[] Vertices { init; get; }
            internal Edge[] Edges { init; get; }
        }
        
        // DO NOT modify innerGrid
        private HexGrid<InnerCell> InnerGrid { get; init; }
        public float DistanceFromCenter { get; init; }
        
        public delegate Vertex VertexFactory(Cell cell, HexPosition hex, HexVertexDir dir);
        public delegate Edge EdgeFactory(Cell cell, HexPosition hex, HexEdgeDir dir);

        private readonly VertexFactory _vertexFactory;
        private readonly EdgeFactory _edgeFactory;

        public FullHexGrid(int width, int height, VertexFactory vertexFactory, EdgeFactory edgeFactory, float distanceFromCenter = 6f)
        {
            InnerGrid = new HexGrid<InnerCell>(width, height, distanceFromCenter);
            DistanceFromCenter = distanceFromCenter;
            this._vertexFactory = vertexFactory;
            this._edgeFactory = edgeFactory;
        }
        
        [CanBeNull] 
        public Cell this[HexPosition position]
        {
            get => InnerGrid[position]?.Cell;
            
            set
            {
                var innerCell = InnerGrid[position];
                if (innerCell is null)
                {
                    // a new inner cell needs to be created
                    var vertices = new Vertex[6];
                    var edges = new Edge[6];
                    
                    var newInnerCell = new InnerCell
                    {
                        Cell = value,
                        Vertices = vertices,
                        Edges = edges,
                    };

                    // look at neighbours to see if edges and vertices have already been init
                    var neighbours = position.GetNeighbours().Select(p => InnerGrid[p]).ToArray();
                    
                    for (int i = 0; i < 6; i++) {
                        // this is sketchy af but I am mostly sure it's correct :3e
                        edges[i] = neighbours[i]?.Edges[(i + 3) % 6];
                        vertices[i] = (neighbours[i]?.Vertices[(i + 2) % 6]) ?? (neighbours[(i+1) % 6]?.Vertices[(i + 4) % 6]);
                    }
                    
                    // init leftover vertices and edges
                    for (int i = 0; i < 6; i++)
                    {
                        vertices[i] ??= _vertexFactory(value, position, (HexVertexDir) i);
                        edges[i] ??= _edgeFactory(value, position, (HexEdgeDir) i);
                    }
                    
                    InnerGrid[position] = newInnerCell;
                }
                else
                    // doar updatam celula user-ului
                    innerCell.Cell = value;
            }
        }
        
        // returns null if the cell has not been init
        [CanBeNull]
        public Vertex GetVertex(HexPosition position, HexVertexDir dir) 
            => InnerGrid[position]?.Vertices[(int)dir];
        
        // returns 6 vertices (if the cell has been init)
        public IEnumerable<(Vertex, HexVertexDir)> GetHexVertices(HexPosition position)
            => Enumerable.Zip(
                    InnerGrid[position]?.Vertices.AsEnumerable() ?? Enumerable.Empty<Vertex>(), 
                    Enumerable.Range(0, 6),
                    (v, i) => (v, (HexVertexDir)i)
                );
        
        // returns up to 3 vertices (if they have all been initialised)
        // *
        //  \
        //   * - *
        //  /
        // *
        public IEnumerable<(Vertex, HexPosition, HexVertexDir)> GetNeighbouringVertices(HexPosition position, HexVertexDir dir)
        {
            var innerCell = InnerGrid[position];
            if (innerCell is null) yield break;

            // first two vertices are on the same hex so its ez
            int v1 = ((int)dir + 1) % 6;
            yield return (innerCell.Vertices[v1], position, (HexVertexDir)v1);
            
            int v2 = ((int)dir + 5) % 6;
            yield return (innerCell.Vertices[v2], position, (HexVertexDir)v2);
            
            // to get a reference to the last vertexes we need to look at two neighbouring cells (bc one might not be init)
            HexPosition leftCellPos = position.GetNeighbour((HexEdgeDir) dir);
            HexPosition rightCellPos = position.GetNeighbour((HexEdgeDir) (((int)dir + 1) % 6));
            
            var leftDir = ((int)dir + 1) % 6;
            var rightDir = ((int) dir + 5) % 6;
            
            var leftVertex = InnerGrid[leftCellPos]?.Vertices[leftDir];
            var rightVertex = InnerGrid[rightCellPos]?.Vertices[rightDir];

            if (leftVertex is null && rightVertex is null) yield break;
            
            // sanity check
            if (leftVertex is not null && rightVertex is not null && ReferenceEquals(leftVertex, rightVertex))
                Debug.LogError("Left and right vertices are not the same instance in GetNeighbouringVertices, something has gone terribly wrong!");

            if (leftVertex is not null)
                yield return (leftVertex, leftCellPos, (HexVertexDir) leftDir);
            else
                yield return (rightVertex, rightCellPos, (HexVertexDir) rightDir);
        }
        
        
        // returns null if the cell has not been init
        [CanBeNull]
        public Edge GetEdge(HexPosition position, HexEdgeDir dir)
            => InnerGrid[position]?.Edges[(int)dir];
        
        // returns 6 edges (if the cell has been init)
        public IEnumerable<(Edge, HexEdgeDir)> GetHexEdges(HexPosition position)
            => Enumerable.Zip(
                InnerGrid[position]?.Edges.AsEnumerable() ?? Enumerable.Empty<Edge>(), 
                Enumerable.Range(0, 6),
                (v, i) => (v, (HexEdgeDir)i)
            );

        // returns up to 4 edges (if they have all been initialised)
        //    \     /
        //     * - *
        //    /     \
        public IEnumerable<(Vertex, HexPosition, HexVertexDir)> GetNeighbouringEdges(HexPosition position, HexEdgeDir dir)
        {
            var innerCell = InnerGrid[position];
            if (innerCell is null) yield break;

            // first two edges are on the same hex so its ez
            int v1 = ((int)dir + 1) % 6; 
            yield return (innerCell.Vertices[v1], position, (HexVertexDir)v1);
            
            int v2 = ((int)dir + 5) % 6;
            yield return (innerCell.Vertices[v2], position, (HexVertexDir)v2);
            
            // like with GetNeighbouringVertices, we need to look at potentially multiple cells
            var middleCellPos = position.GetNeighbour(dir);
            var middleCell = InnerGrid[middleCellPos]; 
            if (middleCell is not null)
            {
                var v3 = ((int) dir + 4) % 6;
                yield return (middleCell.Vertices[v3], middleCellPos, (HexVertexDir)v3);
                
                var v4 = ((int) dir + 2) % 6;
                yield return (middleCell.Vertices[v4], middleCellPos, (HexVertexDir)v4);
            } 
            else
            {
                // middle hex cell is not init so we look for the ones to the left and right
                var leftCellPos = position.GetNeighbour((HexEdgeDir) (((int)dir + 5) % 6) );
                var leftCell = InnerGrid[leftCellPos];

                if (leftCell is not null)
                {
                    var leftDir = ((int) dir + 1) % 6;
                    yield return (leftCell.Vertices[leftDir], leftCellPos, (HexVertexDir)leftDir);
                }
                
                var rightCellPos = position.GetNeighbour((HexEdgeDir) (((int)dir + 1) % 6));
                var rightCell = InnerGrid[rightCellPos];

                if (rightCell is not null)
                {
                    var rightDir = ((int) dir + 5) % 6;
                    yield return (rightCell.Vertices[rightDir], rightCellPos, (HexVertexDir)rightDir);
                } 
            }
        }
        
        public Vector2 ToWorldPosition(HexPosition position) => 
            DistanceFromCenter * new Vector2(position.X * 1.5f, MathF.Sqrt(3) * (position.Y + (float)position.X / 2));
        
        public Vector2 GetHexCorner(HexVertexDir dir, HexPosition position)
        {
            var hexCenter = ToWorldPosition(position);
            return hexCenter + dir.OffsetFromCenter() * DistanceFromCenter;
        }
        
        public Vector2 GetHexEdge(HexEdgeDir dir, HexPosition position)
        {
            var hexCenter = ToWorldPosition(position);
            return hexCenter + dir.OffsetFromCenterOfHex() * DistanceFromCenter;
        }
    }
}