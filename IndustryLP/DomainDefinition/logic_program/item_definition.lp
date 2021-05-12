#const rows = 2.
#const cols = 2.
#const parcels = 4.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%% Definimos los ámbitos de las parcelas
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Fila de la cuadrícula
row(0 .. (rows-1)).
% Columna de la cuadrícula
col(0 .. (cols-1)).
% Número de parcela
n_parcel(0 .. 8).