import * as d3 from 'd3';

export function renderPoRGraph(selector: string, porHash: string) {
    const container = d3.select(selector);
    container.selectAll('*').remove(); // Apaga traces anteriores (Refresh determinístico)
    
    // Tamanho fluido adaptável
    const width = 800;
    const height = 400;

    const svg = container.append("svg")
        .attr("width", "100%")
        .attr("height", "100%")
        .attr("viewBox", `0 0 ${width} ${height}`);

    // Cria a Topologia Neural mock (4 Camadas LSTMs Ponto Fixo representadas simplificada)
    const nodes = [
        { id: "EVM Smart Wallet (Tx)", x: 100, y: height/2 },
        { id: "LSTM Layer 1 (XAI State)", x: 300, y: height/4 },
        { id: "LSTM Layer 4 (XAI State)", x: 300, y: (height/4)*3 },
        { id: "Neural Output Root", x: 500, y: height/2 },
        { id: `PoR Hash: ${porHash.substring(0, 10)}...`, x: 750, y: height/2 }
    ];

    // Vetores Matemáticos Representacionais
    svg.selectAll("line")
        .data([[0,1],[0,2],[1,3],[2,3],[3,4]])
        .enter().append("line")
        .attr("x1", d => nodes[d[0]].x)
        .attr("y1", d => nodes[d[0]].y)
        .attr("x2", d => nodes[d[1]].x)
        .attr("y2", d => nodes[d[1]].y)
        .attr("stroke", "#3b82f6") // Cor do Trace Azure
        .attr("stroke-width", 2)
        .attr("stroke-dasharray", "5,5"); // Linha pontilhada (Fluxo de Matrizes)

    // Entidades de Célula Neural/Merkle Root
    svg.selectAll("circle")
        .data(nodes)
        .enter().append("circle")
        .attr("cx", d => d.x)
        .attr("cy", d => d.y)
        .attr("r", 15)
        .attr("fill", d => d.id.includes("PoR") ? "#f59e0b" : "#10b981"); // Marca o Hash Final de âmbar

    // Legenda XAI Audit
    svg.selectAll("text")
        .data(nodes)
        .enter().append("text")
        .attr("x", d => d.x)
        .attr("y", d => d.y - 25)
        .attr("text-anchor", "middle")
        .attr("fill", "#f3f4f6")
        .attr("font-family", "sans-serif")
        .attr("font-size", "12px")
        .text(d => d.id);
}
