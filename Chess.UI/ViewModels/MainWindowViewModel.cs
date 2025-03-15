using Chess.Domain;
using Chess.UI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace Chess.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IReadOnlyDictionary<int, ChessPieceData> _chessPieceDataMap = new Dictionary<int, ChessPieceData>
    {
        { 0, new ChessPieceData() },
        { 1, new ChessPieceData() },
        { 2, new ChessPieceData() }
    };

    private int _chessPieceId = 0;

    private readonly ObservableAsPropertyHelper<ChessPieceData?> _chessPieceData;

    private PieceColor _color;

    private string? _initialPosition;

    private string? _moveToPosition;

    public int ChessPieceId { get => _chessPieceId; set => this.RaiseAndSetIfChanged(ref _chessPieceId, value); }

    public ChessPieceData? ChessPieceData => _chessPieceData.Value;

    public PieceColor Color { get => _color; set => this.RaiseAndSetIfChanged(ref _color, value); }

    public string? InitialPosition { get => _initialPosition; set => this.RaiseAndSetIfChanged(ref _initialPosition, value); }

    public string? MoveToPosition { get => _moveToPosition; set => this.RaiseAndSetIfChanged(ref _moveToPosition, value); }

    public ReactiveCommand<Unit, Unit> PlaceOnBoardCommand { get; }

    public ReactiveCommand<Unit, Unit> MoveToCommand { get; }

    private string? _position;

    public string? Position { get => $"Piece position: {_position}"; set => this.RaiseAndSetIfChanged(ref _position, value); }

    private string? _pieceColor;

    public string? PieceColor { get => $"Piece color: {_pieceColor}"; set => this.RaiseAndSetIfChanged(ref _pieceColor, value); }

    public MainWindowViewModel()
    {
        this.WhenAnyValue(x => x.ChessPieceId)
            .Select(x => _chessPieceDataMap[x])
            .ToProperty(this, x => x.ChessPieceData, out _chessPieceData); ;

        this.WhenAnyValue(x => x.ChessPieceData)
            .Subscribe(x =>
            {
                Color = x?.Color ?? Domain.PieceColor.White;
                InitialPosition = x?.InitialPosition;
                MoveToPosition = x?.MoveToPosition;
                PieceColor = x?.ChessPiece?.Color.ToString();
                Position = x?.ChessPiece?.Position.ToNotation();
            });

        this.WhenAnyValue(x => x.Color)
            .Subscribe(x =>
            {
                if (ChessPieceData != null)
                {
                    ChessPieceData.Color = x;
                }
            });

        this.WhenAnyValue(x => x.InitialPosition)
            .Subscribe(x =>
            {
                if (ChessPieceData != null)
                {
                    ChessPieceData.InitialPosition = x;
                }
            });

        this.WhenAnyValue(x => x.MoveToPosition)
            .Subscribe(x =>
            {
                if (ChessPieceData != null)
                {
                    ChessPieceData.MoveToPosition = x;
                }
            });

        PlaceOnBoardCommand = ReactiveCommand.Create(() =>
        {
            if (ChessPieceData == null)
            {
                return;
            }

            if (!PositionParser.TryParse(InitialPosition, out var position))
            {
                return;
            }


            ChessPieceData.ChessPiece = ChessPieceId switch
            {
                0 => new Queen(Color, position),
                1 => new Rook(Color, position),
                2 => new Bishop(Color, position),
                _ => throw new ArgumentException()
            };

            Position = ChessPieceData.ChessPiece.Position.ToNotation();
            PieceColor = Color.ToString();
        });

        MoveToCommand = ReactiveCommand.Create(() =>
        {
            var piece = ChessPieceData?.ChessPiece;

            if (piece == null)
            {
                return;
            }

            if (!PositionParser.TryParse(MoveToPosition, out var position))
            {
                return;
            }

            piece.Move(position);
            Position = piece.Position.ToNotation();
        });
    }

    private void SetColor(Domain.PieceColor color)
    {
        if (ChessPieceData != null)
        {
            ChessPieceData.Color = color;
        }
    }


}
