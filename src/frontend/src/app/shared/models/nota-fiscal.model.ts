export interface ItemNotaFiscal {
  id: number;
  codigoProduto: string;
  quantidade: number;
  valor: number;
}

export interface NotaFiscal {
  id: number;
  numero: number;
  status: 'Aberta' | 'Fechada';
  dataEmissao: string;
  dataFechamento: string | null;
  total: number;
  itens: ItemNotaFiscal[];
}
