import * as React from 'react';
import ColumnFill from './ColumnFill';
import { Board, BoardColumn } from './Interfaces';

export default class Column extends React.Component<{ column: BoardColumn, board: Board, time: number, index: number, columnCount: number }> {
    public render() {
        return <div> <ColumnFill column={this.props.column} board={this.props.board} time={this.props.time} index={this.props.index} columnCount={this.props.columnCount} /> </div>
    }
}
