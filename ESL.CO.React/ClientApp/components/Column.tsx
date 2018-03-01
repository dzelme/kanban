import * as React from 'react';
import ColumnFill from './ColumnFill';
import { Board, BoardColumn } from './Interfaces';

export default class Column extends React.Component<{ column: BoardColumn, board: Board }> {
    public render() {
        return <div> <ColumnFill column={this.props.column} /> </div>
    }
}
