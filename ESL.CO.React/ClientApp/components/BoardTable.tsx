import * as React from 'react';
import ColumnTitle from './ColumnTitle';
import ColumnFill from './ColumnFill';
import { Board } from './Interfaces';

export default class BoardTable extends React.Component<{ board: Board, boardTime: number }> {

    public render() {

        let columnCount = this.props.board.columns.length;

        return <div>
            <tr>{
                this.props.board.columns.map((column, index) =>
                    <th style={this.whichColumnHeader(index)} key={index}><ColumnTitle name={column.name} /></th>
                )
            }</tr>
            <tr>

                {

                    this.props.board.columns.map((column, index) =>

                        <td key={index} style={this.whichColumn(index)}><ColumnFill column={column} board={this.props.board} time={this.props.boardTime} index={index} columnCount={columnCount} /></td>
                    )
                }
            </tr>
        </div>
    }

    whichColumnHeader(Nr: number) {
        let HeaderStyle;

        if (Nr == 0) {
            HeaderStyle = styleColumnNameFirst;
        }
        else {
            HeaderStyle = styleColumnNameOther;
        }

        return HeaderStyle;
    }

    whichColumn(Nr: number) {
        let ColumnStyle;

        if (Nr == 0) {
            ColumnStyle = styleColumnFirst;
        }
        else {
            ColumnStyle = styleColumnOther;
        }

        return ColumnStyle;
    }
}

const styleColumn = {
    border: 'solid',
};

const styleColumnFirst = {
    border: 'solid',
};

const styleColumnOther = {
    borderTop: 'solid',
    borderBottom: 'solid',
    borderRight: 'solid'
};

const styleColumnNameFirst = {
    borderTop: 'solid',
    borderRight: 'solid',
    borderLeft: 'solid'
};

const styleColumnNameOther = {
    borderTop: 'solid',
    borderRight: 'solid'
};