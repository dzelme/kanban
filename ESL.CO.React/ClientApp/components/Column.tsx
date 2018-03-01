import * as React from 'react';
import ColumnTitle from './ColumnTitle';
import ColumnFill from './ColumnFill';
import { Board, BoardColumn } from './Interfaces';

export default class Column extends React.Component<{ column: BoardColumn, board: Board }> {
    public render() {
        let currentColumn = this.props.column;

        return <div>
            <div style={styleColumnTitle}>
                <ColumnTitle name={currentColumn.name} />
            </div>
            <div>
                <ColumnFill column={currentColumn} />
            </div>
        </div>
    }
}

const styleColumnTitle = {
    borderBottom: 'solid'
};